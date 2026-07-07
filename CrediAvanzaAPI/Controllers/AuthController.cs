using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrediAvanzaAPI.Services;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;

namespace CrediAvanzaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DbNegocioContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private static readonly TimeSpan UnlockTokenExpiration = TimeSpan.FromMinutes(15);

    public AuthController(DbNegocioContext context, IEmailService emailService, IConfiguration config)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
    }

    private static string GenerateUnlockToken()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }

    private static bool HasUsuarioRole(IEnumerable<string> roleNames)
    {
        return roleNames.Any(r => string.Equals(r, "Usuario", StringComparison.OrdinalIgnoreCase));
    }

    // Debug endpoint to inspect token claims and roles
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var name = User.Identity?.Name ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return Ok(new { Usuario = name, Claims = claims, Roles = roles });
    }

    // POST: api/auth/reveal-password
    // WARNING: returning decrypted passwords is insecure. This endpoint will only allow administrators and will
    // generate a temporary password instead of returning the stored hash. It will email the user with the temporary password
    // if EnviarCorreo = true. The system does not store passwords in reversible form; stored Password is a BCrypt hash.
    //[Authorize(Roles = "Admin")]
    [HttpPost("reveal-password")]
    public async Task<IActionResult> RevealPassword([FromBody] Request.GenerateTempPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado" });

        // Obtener roles del usuario desde UsuarioRoles -> Role
        var roleNames = await _context.UsuarioRoles
            .Where(ur => ur.IdUsuario == user.IdUsuario)
            .Include(ur => ur.IdRolNavigation)
            .Select(ur => ur.IdRolNavigation.Nombre)
            .ToListAsync();

        // Since passwords are stored as BCrypt hashes (one-way), we cannot decrypt. Generate a temporary password,
        // set it for the user, audit the action, and optionally send it by email.
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        var tempPassword = new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());

        user.Password = BCrypt.Net.BCrypt.HashPassword(tempPassword);
        user.IntentosFallidos = 0;
        user.Bloqueado = 0;
        user.FechaBloqueo = null;
        user.BContrasenaTemporal = true;
        user.DFechaContrasenaTemporal = DateTime.UtcNow;

        _context.UsuarioLogins.Update(user);

        var rolesJoined = roleNames != null && roleNames.Count > 0 ? string.Join(", ", roleNames) : "(sin roles)";

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = $"Se generó contraseña temporal por administrador. Roles asignados: {rolesJoined}"
        });

        await _context.SaveChangesAsync();

        if (request.EnviarCorreo)
        {
            var correo = user.CCorreo;
            var subject = "Contraseña temporal generada";
            var nombre = user.CDocumento;
            var body = $"Su contraseña temporal es: {tempPassword}. Por favor cámbiela en su primer ingreso." +
                       $"\nRoles asignados: {rolesJoined}";
            await _emailService.SendAsync(correo, subject, body);
        }

        // Do not return the stored hash. Return only a success message; temp password is sent by email.
        return Ok(new { Exito = true, Mensaje = request.EnviarCorreo ? "Contraseña temporal enviada por correo" : "Contraseña temporal generada (no enviada)", Roles = roleNames });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] Request.ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.ContrasenaActual) || string.IsNullOrWhiteSpace(request.ContrasenaNueva))
            return BadRequest(new { Exito = false, Mensaje = "Usuario, ContraseñaActual y ContraseñaNueva son obligatorios." });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado" });

        // Verificar contraseña actual
        if (!BCrypt.Net.BCrypt.Verify(request.ContrasenaActual, user.Password))
        {
            user.IntentosFallidos = (user.IntentosFallidos ?? 0) + 1;

            if (user.IntentosFallidos >= 3)
            {
                user.Bloqueado = 1;
                user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));

                // Registrar motivo de bloqueo en tabla de auditoría
                await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
                {
                    IdUsuario = user.IdUsuario,
                    IdPersona = user.IdPersona,
                    Usuario = user.CDocumento,
                    Exito = false,
                    FechaAttempt = DateTime.UtcNow,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IntentosFallidos = user.IntentosFallidos,
                    Bloqueado = true,
                    FechaBloqueo = DateTime.UtcNow,
                    MotivoBloqueo = "Tres intentos fallidos",
                    Observacion = "Cuenta bloqueada por intentos fallidos en cambio de contraseña"
                });
            }

            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();

            return BadRequest(new { Exito = false, Mensaje = "Contraseña actual incorrecta" });
        }

        // Contraseña actual correcta: actualizar
        user.IntentosFallidos = 0;
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.ContrasenaNueva);
        user.UltimoLogin = DateTime.UtcNow; // usar como fecha de modificación
        user.BContrasenaTemporal = false;
        user.DFechaContrasenaTemporal = null;

        _context.UsuarioLogins.Update(user);

        // Registrar auditoría en PasswordChangeAudits
        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = "Cambio de contraseña exitoso"
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Contraseña actualizada correctamente" });
    }

    // POST: api/auth/unlock
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockUser([FromBody] Request.UnlockUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        // Reset bloqueo..
        user.Bloqueado = 0;
        user.IntentosFallidos = 0;
        user.FechaBloqueo = null;

        _context.UsuarioLogins.Update(user);

        // Registrar auditoría de desbloqueo
        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = string.IsNullOrWhiteSpace(request.Observacion) ? "Usuario desbloqueado por administrador" : request.Observacion
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Usuario desbloqueado correctamente" });
    }

    // POST: api/auth/request-unlock-app
    // Solicita un token de desbloqueo para usuarios con rol Usuario y lo envía al correo registrado.
    [HttpPost("request-unlock-app")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestUnlockFromApp([FromBody] Request.UnlockAppRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { Exito = false, Mensaje = "Usuario es requerido" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        var roles = await _context.UsuarioRoles
            .Where(ur => ur.IdUsuario == user.IdUsuario)
            .Include(ur => ur.IdRolNavigation)
            .Select(ur => ur.IdRolNavigation.Nombre)
            .ToListAsync();

        if (!HasUsuarioRole(roles))
            return Unauthorized(new { Exito = false, Mensaje = "Solo los usuarios con rol Usuario pueden solicitar el desbloqueo desde la app." });

        if (user.Bloqueado != 1)
            return BadRequest(new { Exito = false, Mensaje = "El usuario no está bloqueado." });

        if (string.IsNullOrWhiteSpace(user.CCorreo))
            return BadRequest(new { Exito = false, Mensaje = "El usuario no tiene correo registrado para recibir el token." });

        var token = GenerateUnlockToken();

        user.Token = int.Parse(token);
        user.TokenTime = DateTime.UtcNow;
        user.TokenCheck = false;

        _context.UsuarioLogins.Update(user);

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = true,
            Observacion = "Se generó token de desbloqueo desde la app"
        });

        await _context.SaveChangesAsync();

        var subject = "Token de desbloqueo de cuenta";
        var body = $"Estimado(a) usuario:\n\n" +
                   $"Hemos generado un token para desbloquear su cuenta: {token}.\n" +
                   $"Este código vence en {UnlockTokenExpiration.TotalMinutes:0} minutos.\n\n" +
                   "Por favor, ingréselo en la aplicación para completar el proceso.";

        await _emailService.SendAsync(user.CCorreo, subject, body);

        return Ok(new { Exito = true, Mensaje = "Se envió un token de desbloqueo al correo registrado." });
    }

    // POST: api/auth/confirm-unlock-app
    // Confirma el desbloqueo usando el token enviado por correo.
    [HttpPost("confirm-unlock-app")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmUnlockFromApp([FromBody] Request.ConfirmUnlockAppRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Token))
            return BadRequest(new { Exito = false, Mensaje = "Usuario y Token son obligatorios" });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        var roles = await _context.UsuarioRoles
            .Where(ur => ur.IdUsuario == user.IdUsuario)
            .Include(ur => ur.IdRolNavigation)
            .Select(ur => ur.IdRolNavigation.Nombre)
            .ToListAsync();

        if (!HasUsuarioRole(roles))
            return Unauthorized(new { Exito = false, Mensaje = "Solo los usuarios con rol Usuario pueden usar este flujo." });

        if (user.Bloqueado != 1)
            return BadRequest(new { Exito = false, Mensaje = "El usuario no está bloqueado." });

        if (user.Token == null || user.TokenTime == null)
            return BadRequest(new { Exito = false, Mensaje = "No existe un token de desbloqueo vigente." });

        if (user.TokenCheck == true)
            return BadRequest(new { Exito = false, Mensaje = "El token ya fue utilizado." });

        if (DateTime.UtcNow - user.TokenTime.Value > UnlockTokenExpiration)
        {
            user.Token = null;
            user.TokenTime = null;
            user.TokenCheck = null;
            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();
            return BadRequest(new { Exito = false, Mensaje = "El token de desbloqueo expiró." });
        }

        if (!int.TryParse(request.Token.Trim(), out var tokenIngresado) || tokenIngresado != user.Token.Value)
            return BadRequest(new { Exito = false, Mensaje = "Token inválido" });

        user.Bloqueado = 0;
        user.IntentosFallidos = 0;
        user.FechaBloqueo = null;
        user.Token = null;
        user.TokenTime = null;
        user.TokenCheck = true;

        _context.UsuarioLogins.Update(user);

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = "Usuario desbloqueado desde la app con token válido"
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Usuario desbloqueado correctamente" });
    }

    [HttpPost("change-temp-password")]
    public async Task<IActionResult> ChangeTempPassword([FromBody] Request.ChangeTempPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.ContrasenaNueva))
            return BadRequest(new { Exito = false, Mensaje = "Usuario y ContraseñaNueva son obligatorios." });

        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == request.Usuario);
        if (user == null)
            return NotFound(new { Exito = false, Mensaje = "Usuario no existe" });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado" });

        // Ensure this is a temporary password flow
        if (user.BContrasenaTemporal != true)
            return BadRequest(new { Exito = false, Mensaje = "El usuario no tiene contraseña temporal" });

        // Update to new password without requiring the old one (as requested)
        user.IntentosFallidos = 0;
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.ContrasenaNueva);
        user.UltimoLogin = DateTime.UtcNow;
        user.BContrasenaTemporal = false;
        user.DFechaContrasenaTemporal = null;

        _context.UsuarioLogins.Update(user);

        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = "Cambio de contraseña temporal exitoso"
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Contraseña actualizada correctamente", Cambiada = true });
    }

    // New login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { Exito = false, Mensaje = "Documento y contraseña son obligatorios." });

        var documentoRaw = request.Documento.Trim();

        // Try exact match first
        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == documentoRaw);
        if (user == null)
        {
            var normalizedInput = documentoRaw.Replace("-", string.Empty).Replace(" ", string.Empty).ToLower();
            user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => (u.CDocumento ?? string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).ToLower() == normalizedInput);
        }

        if (user == null)
            return Unauthorized(new { Exito = false, Mensaje = "Usuario o contraseña inválidos." });

        if (user.Bloqueado == 1)
            return BadRequest(new { Exito = false, Mensaje = "Usuario bloqueado." });

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            user.IntentosFallidos = (user.IntentosFallidos ?? 0) + 1;

            if (user.IntentosFallidos >= 3)
            {
                user.Bloqueado = 1;
                user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));

                await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
                {
                    IdUsuario = user.IdUsuario,
                    IdPersona = user.IdPersona,
                    Usuario = user.CDocumento,
                    Exito = false,
                    FechaAttempt = DateTime.UtcNow,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IntentosFallidos = user.IntentosFallidos,
                    Bloqueado = true,
                    FechaBloqueo = DateTime.UtcNow,
                    MotivoBloqueo = "Tres intentos fallidos",
                    Observacion = "Cuenta bloqueada por intentos fallidos en login"
                });
            }

            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();

            return Unauthorized(new { Exito = false, Mensaje = "Usuario o contraseña inválidos." });
        }

        // Successful authentication
        user.IntentosFallidos = 0;
        user.UltimoLogin = DateTime.UtcNow;
        _context.UsuarioLogins.Update(user);

        // create claims including roles
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.CDocumento),
            new Claim("IdPersona", user.IdPersona.ToString())
        };

        var roles = await _context.UsuarioRoles.Where(ur => ur.IdUsuario == user.IdUsuario).Include(ur => ur.IdRolNavigation).Select(ur => ur.IdRolNavigation.Nombre).ToListAsync();
        // Only allow login for users with role 'Usuario' or 'Usuario estándar'
        var allowedRoles = new[] { "Usuario", "Usuario estándar" };
        var hasAllowedRole = roles != null && roles.Any(r => allowedRoles.Contains(r));
        if (!hasAllowedRole)
        {
            // Audit denied login due to missing allowed role
            await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
            {
                IdUsuario = user.IdUsuario,
                IdPersona = user.IdPersona,
                Usuario = user.CDocumento,
                Exito = false,
                FechaAttempt = DateTime.UtcNow,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IntentosFallidos = user.IntentosFallidos,
                Bloqueado = false,
                Observacion = "Intento de login rechazado: rol no permitido"
            });
            await _context.SaveChangesAsync();

            // Return a clear unauthorized message when the user has no allowed role
            return Unauthorized(new { Exito = false, Mensaje = "Usuario no tiene rol asignado o rol no permitido." });
        }

        var jwtKey = _config["Jwt:Key"] ?? "ChangeThisSecretInProduction_ReplaceMeWithStrongKey";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // audit success
        await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
        {
            IdUsuario = user.IdUsuario,
            IdPersona = user.IdPersona,
            Usuario = user.CDocumento,
            Exito = true,
            FechaAttempt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers["User-Agent"].ToString(),
            IntentosFallidos = user.IntentosFallidos,
            Bloqueado = false,
            Observacion = "Login exitoso"
        });

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, 
                        Mensaje = "Autenticación exitosa", 
                        Token = tokenString, 
                        IdPersona = user.IdPersona, 
                        bTemporal = user.BContrasenaTemporal == true });
    }
}
