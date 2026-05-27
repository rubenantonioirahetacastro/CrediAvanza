using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuscarClienteController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public BuscarClienteController(DbNegocioContext context)
        {
            _context = context;
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

            if (!existe)
            {
                return Ok(null);
            }

            return Ok(new { message = "El documento ingresado ya posee un credito o solicitud en proceso, favor iniciar sesión o recuperar contraseña" });
        }
    }
}
