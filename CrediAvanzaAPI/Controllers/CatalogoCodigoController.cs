using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogoCodigoController : ControllerBase
    {
        private readonly ICatalogoCodigoService _catalogoCodigoService;

        public CatalogoCodigoController(ICatalogoCodigoService catalogoCodigoService)
        {
            _catalogoCodigoService = catalogoCodigoService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCatalogo([FromBody] Models.CatalogoCodigo catalogo)
        {
            try
            {
                await _catalogoCodigoService.AddCatalogo(catalogo);

                return CreatedAtAction(
                    nameof(GetCatalogoById),
                    new { codigo = catalogo.NCodigo },
                    catalogo
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllCatalogos()
        {
            var catalogos = await _catalogoCodigoService.AllCatalogos();
            return Ok(catalogos);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetCatalogoById(int codigo)
        {
            var catalogos = await _catalogoCodigoService.GetCatalogoById(codigo);

            if (catalogos == null || !catalogos.Any())
                return NotFound("No existen valores para este catálogo");

            return Ok(catalogos);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCatalogo([FromBody] Models.CatalogoCodigo catalogo)
        {
            var updated = await _catalogoCodigoService.UpdateCatalogo(catalogo);

            if (!updated)
                return NotFound("El catálogo no existe");

            return NoContent(); // 204
        }

        [HttpDelete("{codigo}/{valor}")]
        public async Task<IActionResult> DeleteCatalogo(int codigo, int valor)
        {
            var deleted = await _catalogoCodigoService.DeleteCatalogo(codigo, valor);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}