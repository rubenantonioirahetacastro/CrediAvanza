using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCreditoController : ControllerBase
    {
        private readonly ISolicitudCreditoService _service;

        public SolicitudCreditoController(ISolicitudCreditoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SolicitudCreditoRequest? request)
        {
            if (request == null)
                return BadRequest("Body requerido");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Soporte para múltiples garantías con fotos (nuevo formato)
            // Si no viene la lista, mantenemos el comportamiento anterior (una garantía).
            var garantias = request.Garantias != null && request.Garantias.Any()
                ? request.Garantias
                : new List<GarantiaConFotosRequest>
                {
                    new()
                    {
                        Garantia = request.Garantia,
                        Fotos = request.FotoGarantiums
                    }
                };

            var filas = await _service.CrearSolicitudAsync(
                request.FotoIds,
                request.FotoDocumentacions,
                request.FotoNegocios,
                garantias,
                request.Persona,
                request.Conyuge,
                request.Fiador,
                request.Negocio,
                request.Compra,
                request.Venta,
                request.Credito
            );

            return Ok(new { filasAfectadas = filas });
        }
    }
}
