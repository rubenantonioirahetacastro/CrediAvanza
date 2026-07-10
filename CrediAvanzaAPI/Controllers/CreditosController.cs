using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Response;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/creditos")]
    [ApiController]
    public class CreditosController : ControllerBase
    {
        private readonly ISimulacionCalendarioService _simulacionCalendarioService;

        public CreditosController(ISimulacionCalendarioService simulacionCalendarioService)
        {
            _simulacionCalendarioService = simulacionCalendarioService;
        }

        [HttpPost("simular-calendario")]
        public async Task<ActionResult<SimularCalendarioResponse>> SimularCalendario([FromBody] SimularCalendarioRequest request)
        {
            try
            {
                var response = await _simulacionCalendarioService.SimularAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}
