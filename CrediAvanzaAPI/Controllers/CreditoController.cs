using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Response;
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
        private readonly ISimulacionCalendarioService _simulacionCalendarioService;

        public CreditoController(
            DbNegocioContext context,
            ICatalogoCodigoService catalogoCodigoService,
            ISimulacionCalendarioService simulacionCalendarioService)
        {
            _context = context;
            _catalogoCodigoService = catalogoCodigoService;
            _simulacionCalendarioService = simulacionCalendarioService;
        }

        [HttpGet("listaCreditos-cpc")]
        public async Task<IActionResult> ListarCreditos([FromQuery] int? ncodage)
        {
            try
            {
                var estados = new[] { 1, 2, 3 };

                var query = _context.Creditos.AsQueryable()
                    .Where(c => estados.Contains(c.NEstado));

                if (ncodage.HasValue)
                {
                    query = query.Where(c => c.NCodAge == ncodage.Value);
                }

                var lista = await (from c in query
                                   join p in _context.Personas on c.IdPersona equals p.IdPersona into ps
                                   from p in ps.DefaultIfEmpty()
                                   join a in _context.Agencias on c.NCodAge equals a.NCodAge into ags
                                   from a in ags.DefaultIfEmpty()
                                   join cat in _context.CatalogoCodigos on new { codigo = 116, valor = c.NEstado } equals new { codigo = cat.NCodigo, valor = cat.NValor } into cats
                                   from cat in cats.DefaultIfEmpty()
                                   join catsub in _context.CatalogoCodigos on new { codigo = 109, valor = (int?)c.NSubProd } equals new { codigo = catsub.NCodigo, valor = (int?)catsub.NValor } into catsubs
                                   from catsub in catsubs.DefaultIfEmpty()
                                   select new CrediAvanzaAPI.Response.CreditoListadoResponse
                                   {
                                       NCodCred = c.NCodCred,
                                       NCodAge = c.NCodAge,
                                       Agencia = a == null ? null : a.CNomAge,
                                       MontoSolicitado = c.NPrestamo,
                                       Estado = cat == null ? null : cat.CNomCod,
                                       NSubProd = c.NSubProd,
                                       SubProducto = catsub == null ? null : catsub.CNomCod,
                                       NombreCliente = p == null ? null : (p.CNombres + " " + p.CPrimerApellido + " " + (p.CSegundoApellido ?? "")).Trim()
                                   }).ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
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

        [HttpGet("Credito/Persona/{nIdPersona}/Ultimo")]
        public async Task<IActionResult> ObtenerUltimoCreditoPorPersona(int nIdPersona)
        {
            try
            {
                var credito = await _context.Creditos
                    .Where(x => x.IdPersona == nIdPersona)
                    .OrderByDescending(x => x.DFecVig)
                    .ThenByDescending(x => x.NCodCred)
                    .FirstOrDefaultAsync();

                if (credito == null)
                {
                    return NotFound(new { Mensaje = "No se encontró un crédito para la persona indicada." });
                }

                return Ok(await ConstruirUltimoCreditoRespuestaAsync(credito));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno del servidor: {ex.Message}" });
            }
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

            var estado = await _context.CatalogoCodigos
                .Where(x => x.NCodigo == 116 && x.NValor == credito.NEstado)
                .Select(x => x.CNomCod)
                .FirstOrDefaultAsync();


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

        private async Task<UltimoCreditoPersonaResponse> ConstruirUltimoCreditoRespuestaAsync(Credito credito)
        {
            var calendarioCond = credito.IdCredCalendCond.HasValue
                ? await _context.CredCalendConds.FirstOrDefaultAsync(c => c.IdCredCalendCond == credito.IdCredCalendCond.Value)
                : null;

            var calendario = await _context.CredCalendarios
                .Where(x => x.NCodAge == credito.NCodAge && x.NCodCred == credito.NCodCred &&
                            (calendarioCond == null || x.NNroCalen == calendarioCond.NNroCalen))
                .OrderBy(x => x.NNroCuota)
                .ToListAsync();

            var estadoCredito = await _catalogoCodigoService.GetCatalogoById(116);
            var descripcionEstado = estadoCredito
                .FirstOrDefault(x => x.NValor == credito.NEstado)
                ?.CNomCod;

            var cuota = calendario.FirstOrDefault()?.NTotalCuota ?? 0m;
            var fechaVencimiento = calendario.Count == 0
                ? (DateTime?)null
                : calendario.Max(x => x.DFecVenc);

            var capitalPagado = calendario.Sum(x => x.NCapPag);
            var cuotasVencidas = calendario.Count(x => x.NEstado != 1 && x.DFecVenc.Date < DateTime.Today.Date);
            var porcentajeCuotasVencidas = credito.NNroCuotas > 0
                ? (decimal)cuotasVencidas / credito.NNroCuotas
                : 0m;

            if (!EsCreditoConFechaVencimientoVisible(credito.NEstado, descripcionEstado))
            {
                fechaVencimiento = null;
            }

            var pagado = calendario.Sum(x => x.NCapPag + x.NIntPag + x.NIntMorPag + x.NIgvPag);
            var pendiente = calendario.Sum(x => Math.Max(0m, (x.NTotalCuota ?? 0m) - (x.NCapPag + x.NIntPag + x.NIntMorPag + x.NIgvPag)));
            var cuotaspagadas = calendario.Count(x => x.NEstado == 1);
            var cuotasrestante = Math.Max(credito.NNroCuotas - cuotaspagadas, 0);

            return new UltimoCreditoPersonaResponse
            {
                Ultimocred = new UltimoCreditoDetalleResponse
                {
                    Prestamo = credito.NPrestamo,
                    NroCuota = cuota,
                    Ncuotas = credito.NNroCuotas,
                    FechaVencimiento = fechaVencimiento,
                    Ncodcred = credito.NCodCred,
                    BReprestamo = credito.NPrestamo > 0 && (capitalPagado / credito.NPrestamo) >= 0.5m,
                    BRefinanciamiento = porcentajeCuotasVencidas > 0.30m
                },
                Resumenultcred = new ResumenUltimoCreditoResponse
                {
                    Pagado = pagado,
                    Pendiente = pendiente,
                    CuotasRestantes = cuotasrestante,
                    CuotaslTotales = credito.NNroCuotas,
                    CuotasPagadas = cuotaspagadas
                }
            };
        }

        private static bool EsCreditoConFechaVencimientoVisible(int nEstado, string? descripcionEstado)
        {
            if (nEstado <= 30)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(descripcionEstado))
            {
                return false;
            }

            return descripcionEstado.Contains("activo", StringComparison.OrdinalIgnoreCase)
                   || descripcionEstado.Contains("desembols", StringComparison.OrdinalIgnoreCase);
        }
    }
}
