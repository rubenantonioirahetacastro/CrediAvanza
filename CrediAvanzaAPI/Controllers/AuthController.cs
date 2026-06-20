using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CrediAvanzaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DbNegocioContext _context;

    public AuthController(DbNegocioContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Documento) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Documento y contraseña son obligatorios.");

            var user = await _context.UsuarioLogins
                .FirstOrDefaultAsync(u => u.CDocumento == request.Documento);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

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

                // Bloquear si alcanza 3 intentos
                if (user.IntentosFallidos >= 3)
                {
                    user.Bloqueado = 1;
                    // Guardar la fecha de bloqueo como entero YYYYMMDD
                    user.FechaBloqueo = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                }

                _context.UsuarioLogins.Update(user);
                await _context.SaveChangesAsync();

                return Unauthorized("Credenciales inválidas.");
            }

            // Credenciales correctas: actualizar UltimoLogin y resetear intentos fallidos
            user.UltimoLogin = DateTime.UtcNow;
            user.IntentosFallidos = 0;
            _context.UsuarioLogins.Update(user);
            await _context.SaveChangesAsync();

            // Devolver el IdPersona (código del cliente)
            return Ok(new { codigoCliente = user.IdPersona });
    }
}
