using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;
using CrediAvanzaAPI.Request;
using Microsoft.AspNetCore.Authorization;

namespace CrediAvanzaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DbNegocioContext _context;

    public TokenController(IConfiguration config, DbNegocioContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Trim and normalize input to avoid mismatches due to formatting (hyphens/spaces)
        var documentoRaw = request.Documento?.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(documentoRaw) || string.IsNullOrWhiteSpace(password))
            return BadRequest("Documento y contraseña son obligatorios.");

        // Try exact match first
        var user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == documentoRaw);

        if (user == null)
        {
            // Try normalized match: remove hyphens and spaces and compare case-insensitively
            var normalizedInput = documentoRaw.Replace("-", string.Empty).Replace(" ", string.Empty).ToLower();
            user = await _context.UsuarioLogins.FirstOrDefaultAsync(u => (u.CDocumento ?? string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).ToLower() == normalizedInput);
        }

        if (user == null) return Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return Unauthorized();

        // crear claims, incluir roles
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.CDocumento),
            new Claim("IdPersona", user.IdPersona.ToString())
        };

        var roles = await _context.UsuarioRoles.Where(ur => ur.IdUsuario == user.IdUsuario).Include(ur => ur.IdRolNavigation).Select(ur => ur.IdRolNavigation.Nombre).ToListAsync();
        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
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

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // Helper to create a user and assign roles.
    // This endpoint allows creating users without requiring authentication.
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Documento y Password son obligatorios.");

        var documento = request.Documento.Trim();

        // Check if user already exists
        var existing = await _context.UsuarioLogins.FirstOrDefaultAsync(u => u.CDocumento == documento);
        if (existing != null)
            return Conflict("Usuario ya existe.");

        // Create new person id placeholder (set to 0)
        var newUser = new UsuarioLogin
        {
            IdPersona = 0,
            CDocumento = documento,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CCorreo = request.Correo ?? string.Empty,
            Token = null,
            TokenTime = null,
            TokenCheck = false,
            Estado = 1,
            IntentosFallidos = 0,
            Bloqueado = 0,
            UltimoLogin = null,
            BContrasenaTemporal = false,
            DFechaContrasenaTemporal = null
        };

        await _context.UsuarioLogins.AddAsync(newUser);
        await _context.SaveChangesAsync();

        // Assign roles: create role if doesn't exist
        foreach (var r in request.Roles ?? Array.Empty<string>())
        {
            var roleName = r.Trim();
            if (string.IsNullOrEmpty(roleName)) continue;
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Nombre == roleName);
            if (role == null)
            {
                role = new Role
                {
                    Nombre = roleName,
                    Descripcion = null,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();
            }

            var userRole = new UsuarioRole
            {
                IdUsuario = newUser.IdUsuario,
                IdRol = role.IdRol,
                FechaAsignacion = DateTime.UtcNow
            };
            await _context.UsuarioRoles.AddAsync(userRole);
        }

        await _context.SaveChangesAsync();

        return Ok(new { Mensaje = "Usuario creado" });
    }
}
