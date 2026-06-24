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

        //Endpoind 

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
    }
}
