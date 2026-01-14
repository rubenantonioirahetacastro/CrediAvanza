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
        public async Task<IActionResult> Crear([FromBody] SolicitudCreditoRequest request)
        {
            var filas = await _service.CrearSolicitudAsync(
                request.Foto, request.Persona, request.Compra, request.Conyuge, request.Documentacion, request.Fiador,
                request.Garantia, request.Venta, request.Credito
            );

            return Ok(new { filasAfectadas = filas });
        }
    }
}
