using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrediAvanzaAPI.Services;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CrediAvanzaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DbNegocioContext _context;
    private readonly IEmailService _emailService;

    public AuthController(DbNegocioContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // POST: api/auth/reveal-password
    // WARNING: returning decrypted passwords is insecure. This endpoint will only allow administrators and will
    // generate a temporary password instead of returning the stored hash. It will email the user with the temporary password
    // if EnviarCorreo = true. The system does not store passwords in reversible form; stored Password is a BCrypt hash.
    [Authorize(Roles = "Admin")]
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
            Observacion = "Se generó contraseña temporal por administrador"
        });

        await _context.SaveChangesAsync();

        if (request.EnviarCorreo)
        {
            var correo = user.CCorreo;
            var subject = "Contraseña temporal generada";
            var nombre = user.CDocumento;
            var body = $"Su contraseña temporal es: {tempPassword}. Por favor cámbiela en su primer ingreso.";
            await _emailService.SendAsync(correo, subject, body);
        }

        // Do not return the stored hash. Return only a success message; temp password is sent by email.
        return Ok(new { Exito = true, Mensaje = request.EnviarCorreo ? "Contraseña temporal enviada por correo" : "Contraseña temporal generada (no enviada)" });
    }

    [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Documento y contraseña son obligatorios.");

            var user = await _context.UsuarioLogins
                .FirstOrDefaultAsync(u => u.CDocumento == request.Documento);

            if (user == null)
            {
                // Registrar intento fallido para usuario inexistente
                await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
                {
                    IdUsuario = null,
                    IdPersona = null,
                    Usuario = request.Documento,
                    Exito = false,
                    FechaAttempt = DateTime.UtcNow,
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Observacion = "Usuario no existe"
                });
                await _context.SaveChangesAsync();

                return Unauthorized("Credenciales inválidas.");
            }

            // Validar estado y bloqueo
            if (user.Estado != 1 || user.Bloqueado == 1)
            {
                return Unauthorized("Usuario inactivo o bloqueado.");
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                // Incrementar intentos fallidos
                user.IntentosFallidos = (user.IntentosFallidos ?? 0) + 1;

                // Determinar si se bloquea
                var bloqueado = false;
                if (user.IntentosFallidos >= 3)
                {
                    user.Bloqueado = 1;
                    user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                    bloqueado = true;
                }

                _context.UsuarioLogins.Update(user);

                // Registrar auditoría de login fallido
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
                    Bloqueado = bloqueado,
                    FechaBloqueo = bloqueado ? DateTime.UtcNow : (DateTime?)null,
                    MotivoBloqueo = bloqueado ? "Tres intentos fallidos" : null,
                    Observacion = "Credenciales inválidas"
                });

                await _context.SaveChangesAsync();

                return Unauthorized("Credenciales inválidas.");
            }

            // Credenciales correctas: si la contraseña temporal existe validar expiración
            if (user.BContrasenaTemporal == true)
            {
                var fechaTemp = user.DFechaContrasenaTemporal;
                if (!fechaTemp.HasValue || DateTime.UtcNow > fechaTemp.Value.AddHours(12))
                {
                    // registrar auditoría de intento con contraseña temporal expirada
                    await _context.PasswordChangeAudits.AddAsync(new PasswordChangeAudit
                    {
                        IdUsuario = user.IdUsuario,
                        IdPersona = user.IdPersona,
                        Usuario = user.CDocumento,
                        Exito = false,
                        FechaAttempt = DateTime.UtcNow,
                        Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = Request.Headers["User-Agent"].ToString(),
                        Observacion = "Contraseña temporal expirada"
                    });
                    await _context.SaveChangesAsync();

                    return Unauthorized(new { Exito = false, Mensaje = "Contraseña temporal expirada. Solicite una nueva contraseña temporal." });
                }
            }

            // Credenciales correctas: actualizar UltimoLogin y resetear intentos fallidos
            user.UltimoLogin = DateTime.UtcNow;
            // si la contraseña es temporal y el usuario ingresó con la temporal, dejamos BContrasenaTemporal = true hasta que cambie la contraseña
            user.IntentosFallidos = 0;
            _context.UsuarioLogins.Update(user);

            // Registrar auditoría de login exitoso
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

            // Devolver el IdPersona (código del cliente) siempre con la misma estructura
            var isTemp = user.BContrasenaTemporal == true;
            var mensaje = isTemp
                ? "La contraseña es temporal. El usuario debe cambiarla dentro de las 12 horas."
                : "Usuario autenticado";

            return Ok(new { codigoCliente = user.IdPersona, PasswordTemporal = isTemp, Mensaje = mensaje });
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
}
