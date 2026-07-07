using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpGet("{nSubProd}")]
        public async Task<IActionResult> GetCambioProducto(int nSubProd)
        {
            if (nSubProd <= 0)
                return BadRequest("El código de subproducto (nSubProd) debe ser mayor a 0.");

            var opciones = await (
                from lca in _context.LineaCatalogoAuxiliars.AsNoTracking()
                join cl in _context.CredLineaCreditos.AsNoTracking()
                    on new { NProd = lca.NProd, NSubProd = lca.NSubProd }
                    equals new { NProd = (int?)cl.NProd, NSubProd = (int?)cl.NSubProd }
                where lca.NProd == 1
                    && lca.NSubProd == nSubProd
                    && lca.NCatalogoCodigo.HasValue
                    && cl.BEstado
                orderby cl.NPlazoMin, cl.NPlazoMax
                select new
                {
                    MontoMin = cl.NMontoMin,
                    MontoMax = cl.NMontoMax,
                    PlazoMin = cl.NPlazoMin,
                    PlazoMax = cl.NPlazoMax,
                    NumeroCatalogo = lca.NCatalogoCodigo,
                    LineaCredito = cl.NCodLinea,
                    Periodicidad = cl.Periodicidad
                })
                .ToListAsync();

            if (opciones == null || opciones.Count == 0)
                return NotFound("No se encontraron opciones de cambio de producto para el subproducto proporcionado.");

            //Meses → Cuotas
            //Cuotas = PlazoMeses * Periodicidad

            //Cuotas → Meses
            //PlazoMeses = Cuotas / Periodicidad

            return Ok(opciones);
        }
    }
}
