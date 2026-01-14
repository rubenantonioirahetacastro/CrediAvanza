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
            if (catalogo == null)
                return BadRequest("El catálogo es inválido");

            var result = await _catalogoCodigoService.AddCatalogo(catalogo);

            if (result == 0)
                return BadRequest("No se pudo guardar el catálogo");

            return CreatedAtAction(nameof(GetCatalogoById), new { id = catalogo.NCodigo }, catalogo);
        }

        [HttpGet]
        public async Task<IActionResult> AllCatalogos()
        {
            var catalogos = await _catalogoCodigoService.AllCatalogos();
            return Ok(catalogos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalogoById(int id)
        {
            var catalogo = await _catalogoCodigoService.GetCatalogoById(id);

            if (catalogo == null)
                return NotFound();

            return Ok(catalogo);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCatalogo([FromBody] Models.CatalogoCodigo catalogo)
        {
            if (catalogo == null)
                return BadRequest();

            var result = await _catalogoCodigoService.UpdateCatalogo(catalogo);

            if (result == 0)
                return NotFound("El catálogo no existe");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogo(int id)
        {
            var result = await _catalogoCodigoService.DeleteCatalogo(id);

            if (result == 0)
                return NotFound();

            return Ok(result);
        }
    }
}