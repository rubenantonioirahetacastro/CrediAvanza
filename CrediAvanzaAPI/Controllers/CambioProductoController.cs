using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CambioProductoController : ControllerBase
    {
        private readonly DbNegocioContext _context;

        public CambioProductoController(DbNegocioContext context)
        {
            _context = context;
        }

        // GET api/CambioProducto/1
        [HttpGet("{nProd}")]
        public async Task<IActionResult> GetOpcionesCambioProducto(int nProd)
        {
            if (nProd <= 0) return BadRequest("El código de producto (nProd) debe ser mayor a 0.");

            var opciones = await _context.CredLineaCreditos
                .AsNoTracking()
                .Where(l => l.NProd == nProd && l.BEstado)
                .OrderBy(l => l.NSubProd)
                .Select(l => new
                {
                    Subproducto = l.NSubProd,
                    Montomin = l.NMontoMin,
                    MontoMax = l.NMontoMax,
                    CatalogoCouta = l.NCodLinea
                })
                .ToListAsync();

            if (opciones == null || opciones.Count == 0)
                return NotFound("No se encontraron opciones de cambio de producto para el producto proporcionado.");

            return Ok(opciones);
        }
    }
}
