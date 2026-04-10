using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarioController : ControllerBase
    {
        private readonly ICalendarioService _calendarioService;

        public CalendarioController(ICalendarioService calendarioService)
        {
            _calendarioService = calendarioService;
        }

        [HttpGet("generar/{nCodAge}/{nCodCred}")]
        public async Task<IActionResult> GenerarCalendario(int nCodAge, int nCodCred)
        {
            try
            {
                var calendario = await _calendarioService.GenerarCalendarioAsync(nCodAge, nCodCred);
                
                if (calendario == null || calendario.Count == 0)
                {
                    return NotFound("No se pudo generar el calendario o no se encontraron cuotas.");
                }

                return Ok(calendario);
            }
            catch (Exception ex)
            {
                // Puedes usar tu ErrorLogger aquí si lo inyectas
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
        [HttpGet("calcular-cuota")]
        public async Task<IActionResult> CalcularCuota([FromQuery] decimal nCapital, [FromQuery] int nPlazo, [FromQuery] int nSubProd, [FromQuery] int nCodAge)
        {
            try
            {
                if (nCapital <= 0 || nPlazo <= 0) return BadRequest("El capital y el plazo deben ser mayores a 0.");
                if (nCodAge <= 0) return BadRequest("El código de agencia (nCodAge) es obligatorio.");

                var calendarioSimulado = await _calendarioService.ProyectarCalendarioAsync(nCapital, nPlazo, nSubProd, nCodAge);
                var cuotaEstimada = calendarioSimulado.FirstOrDefault()?.nTotalCuota ?? 0;

                return Ok(new { CuotaEstimada = cuotaEstimada, Calendario = calendarioSimulado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}