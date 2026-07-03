using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditoController : ControllerBase
    {
        private readonly DbNegocioContext _context;
        private readonly ICatalogoCodigoService _catalogoCodigoService;

        public CreditoController(DbNegocioContext context, ICatalogoCodigoService catalogoCodigoService)
        {
            _context = context;
            _catalogoCodigoService = catalogoCodigoService;
        }

        [HttpGet("Credito/Persona/{idPersona}")]
        public async Task<IActionResult> ObtenerCreditoActivoPorPersona(int idPersona)
        {
            try
            {
                var credito = await _context.Creditos
                    .Where(x => x.IdPersona == idPersona)
                    .OrderByDescending(x => x.DFecVig)
                    .ThenByDescending(x => x.NCodCred)
                    .FirstOrDefaultAsync();

                if (credito == null)
                {
                    return NotFound(new { Mensaje = "No se encontró un crédito para la persona indicada." });
                }

                return Ok(await ConstruirRespuestaCreditoAsync(credito));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
        }

        private async Task<object> ConstruirRespuestaCreditoAsync(Credito credito)
        {
            var persona = credito.IdPersona.HasValue
                ? await _context.Personas.FirstOrDefaultAsync(p => p.IdPersona == credito.IdPersona.Value)
                : null;

            var calendarioCond = credito.IdCredCalendCond.HasValue
                ? await _context.CredCalendConds.FirstOrDefaultAsync(c => c.IdCredCalendCond == credito.IdCredCalendCond.Value)
                : null;

            var calendario = await _context.CredCalendarios
                .Where(x => x.NCodAge == credito.NCodAge && x.NCodCred == credito.NCodCred &&
                            (calendarioCond == null || x.NNroCalen == calendarioCond.NNroCalen))
                .OrderBy(x => x.NNroCuota)
                .ToListAsync();

            var cuotasPendientes = calendario.Count(x => x.NEstado == 0);
            var cuotaProxima = calendario
                .Where(x => x.NEstado == 0)
                .OrderBy(x => x.NNroCuota)
                .FirstOrDefault();

            var catalogoEstados = await _catalogoCodigoService.GetCatalogoById(116);
            var estado = catalogoEstados.FirstOrDefault(x => x.NValor == credito.NEstado)?.CNomCod
                ?? credito.NEstado.ToString();


            return new
            {
                nCodAge = credito.NCodAge,
                nCodCred = credito.NCodCred,
                EstadoCredito = estado,
                numeroReferencia = $"{credito.NCodAge}-{credito.NCodCred}",
                nNroCalen = calendarioCond?.NNroCalen,
                nombreCliente = persona == null
                    ? null
                    : $"{persona.CNombres} {persona.CPrimerApellido} {persona.CSegundoApellido}".Trim(),
                nNroCuotasPendientes = cuotasPendientes,
                cuotaProxima = cuotaProxima == null ? 0m : cuotaProxima.NTotalCuota ?? 0m,
                fechaVencimientoProxima = cuotaProxima?.DFecVenc,
                numeroCuotaProxima = cuotaProxima?.NNroCuota,
                saldoActual = credito.NSaldoK,
                calendarioActual = calendarioCond?.NNroCalen
            };
        }
    }
}
