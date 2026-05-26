using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarPago([FromBody] PagoRequest request)
        {
            try
            {
                var cuotasActualizadas = await _pagoService.RegistrarPagoAsync(request);
                return Ok(new
                {
                    Mensaje = "Pago registrado exitosamente.",
                    CuotasActualizadas = cuotasActualizadas
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}