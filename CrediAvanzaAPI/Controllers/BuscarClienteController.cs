using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CrediAvanzaAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuscarClienteController : ControllerBase
    {
        private readonly DbNegocioContext _context;
        private readonly ErrorLogger _errorLogger;

        public BuscarClienteController(DbNegocioContext context, ErrorLogger errorLogger)
        {
            _context = context;
            _errorLogger = errorLogger;
        }

        [HttpGet("buscarcliente")]
        public async Task<IActionResult> BuscarCliente([FromQuery] string cDocumento)
        {
            if (string.IsNullOrWhiteSpace(cDocumento))
            {
                return BadRequest("Documento es obligatorio.");
            }

            var existe = await (
                from p in _context.Personas.AsNoTracking()
                join c in _context.Creditos.AsNoTracking() on p.IdPersona equals c.IdPersona
                where p.CDocumento == cDocumento
                select 1
            ).AnyAsync();

            // Devolver 1 si existe, 0 si no existe
            return Ok(existe ? 1 : 0);
        }

        [HttpGet("porid/{idPersona}")]
        public async Task<IActionResult> GetClientePorId(int idPersona)
        {
            try
            {
                if (idPersona <= 0) return BadRequest("IdPersona inválido.");

                var persona = await _context.Personas
                    .AsNoTracking()
                    .Where(p => p.IdPersona == idPersona)
                    .Select(p => new
                    {
                        p.IdPersona,
                        p.NTipoDocumento,
                        p.CDocumento,
                        p.DFechaExpedicion,
                        p.DFechaVencimiento,
                        p.NDepartamentoDoc,
                        p.NMunicipioDoc,
                        p.CNombres,
                        p.CPrimerApellido,
                        p.CSegundoApellido,
                        p.NSexo,
                        p.NNacionalidad,
                        p.DFechaNacimiento,
                        p.NDepartamentoNacimiento,
                        p.NMunicipioNacimiento,
                        p.NEstadoCivil,
                        p.NProfesion,
                        p.NEscolaridad,
                        p.CCorreo,
                        p.NTelefono,
                        p.NCelular
                    })
                    .FirstOrDefaultAsync();

                if (persona == null) return NotFound($"Persona {idPersona} no existe en la base de datos");

                var fotos = await _context.FotoIds
                    .AsNoTracking()
                    .Where(f => f.IdPersona == idPersona)
                    .Select(f => new { f.IdFoto, f.VFoto, f.NTipoFoto })
                    .ToListAsync();

                return Ok(new { persona, fotos });
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpGet("usuarioestandar/{idPersona}")]
        public async Task<IActionResult> GetDatosHomeUsuarioPorId(int idPersona)
        {
            try
            {
                if (idPersona <= 0) return BadRequest("IdPersona inválido.");

                var usuario = await (
                    from p in _context.Personas.AsNoTracking()
                    join u in _context.UsuarioLogins.AsNoTracking() on p.IdPersona equals u.IdPersona
                    join ur in _context.UsuarioRoles.AsNoTracking() on u.IdUsuario equals ur.IdUsuario
                    join r in _context.Roles.AsNoTracking() on ur.IdRol equals r.IdRol
                    where p.IdPersona == idPersona && r.Nombre == "Usuario estandar"
                    select new
                    {
                        p.IdPersona,
                        p.CNombres,
                        p.CDocumento,
                        p.CCorreo,
                        NTelefono = (p.NTelefono != 0 ? p.NTelefono.ToString() : p.NCelular.ToString())
                    }
                ).FirstOrDefaultAsync();

                if (usuario == null) return NotFound($"Persona {idPersona} no encontrada o rol no permitido.");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ex);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }
    }
}
