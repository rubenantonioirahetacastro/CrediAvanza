using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Response;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class SimulacionCalendarioService : ISimulacionCalendarioService
    {
        private const decimal TASA_IVA = 0.13m;

        private readonly DbNegocioContext _context;
        private readonly ILineaCreditoService _lineaCreditoService;

        public SimulacionCalendarioService(DbNegocioContext context, ILineaCreditoService lineaCreditoService)
        {
            _context = context;
            _lineaCreditoService = lineaCreditoService;
        }

        public async Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request)
        {
            if (request.Monto <= 0)
                throw new InvalidOperationException("El monto solicitado debe ser mayor a 0.");

            if (request.NPlazo <= 0)
                throw new InvalidOperationException("El plazo debe ser mayor a 0.");

            var linea = await _lineaCreditoService.ResolverLineaCreditoAsync(request.NSubProd, request.Monto);

            if (linea.NProd != request.NProd)
                throw new InvalidOperationException($"La línea de crédito encontrada para el subproducto {request.NSubProd} corresponde al producto {linea.NProd}, no al producto {request.NProd}.");

            if (request.NPlazo < linea.NPlazoMin || request.NPlazo > linea.NPlazoMax)
                throw new InvalidOperationException($"El plazo {request.NPlazo} está fuera del rango permitido para la línea {linea.CLinea} ({linea.NPlazoMin} - {linea.NPlazoMax}).");

            DateTime fechaInicio = request.FechaInicio ?? DateTime.Today;
            decimal tasaNominalMensual = request.TasaOverride ?? linea.NTasaCom;
            decimal gastoPorCuota = request.GastoOverride ?? 0m;

            decimal cuotaFijaEstimada = CalculadoraFinanciera.CalcularCuotaFijaEstimada(
                request.Monto,
                request.NPlazo,
                request.NSubProd,
                tasaNominalMensual,
                TASA_IVA);

            var calendario = CalculadoraFinanciera.GenerarDetalleCuotasSimulado(
                request.Monto,
                request.NPlazo,
                request.NSubProd,
                tasaNominalMensual,
                fechaInicio,
                Array.Empty<DateTime>(),
                cuotaFijaEstimada,
                TASA_IVA,
                gastoPorCuota);

            decimal teaReal = CalculadoraFinanciera.CalcularTeaPorTir(request.Monto, calendario, fechaInicio);

            var (teaMaximaLegal, segmentoLegal, cumpleLeyUsura) = await ObtenerEvaluacionUsuraAsync(request.Monto, fechaInicio, teaReal);

            var cronograma = new List<CuotaDetalleResponse>(calendario.Count);
            decimal saldo = request.Monto;

            foreach (var cuota in calendario)
            {
                decimal totalCuota = cuota.NTotalCuota ?? 0m;
                decimal saldoDespues = saldo - cuota.NCapital;
                if (saldoDespues < 0)
                    saldoDespues = 0;

                cronograma.Add(new CuotaDetalleResponse
                {
                    NroCuota = cuota.NNroCuota,
                    FechaVencimiento = cuota.DFecVenc,
                    Capital = cuota.NCapital,
                    Interes = cuota.NIntComp,
                    Gasto = cuota.Ngasto,
                    Iva = cuota.NIgv,
                    TotalCuota = totalCuota,
                    SaldoDespues = saldoDespues
                });

                saldo = saldoDespues;
            }

            decimal totalCapital = cronograma.Sum(c => c.Capital);
            decimal totalInteres = cronograma.Sum(c => c.Interes);
            decimal totalGasto = cronograma.Sum(c => c.Gasto);
            decimal totalIva = cronograma.Sum(c => c.Iva);
            decimal totalPagado = cronograma.Sum(c => c.TotalCuota);

            return new SimularCalendarioResponse
            {
                LineaUsada = string.IsNullOrWhiteSpace(linea.CDescripcion) ? linea.CLinea : $"{linea.CLinea} - {linea.CDescripcion}",
                TasaNominalMensual = tasaNominalMensual,
                MontoSolicitado = request.Monto,
                Plazo = request.NPlazo,
                Cronograma = cronograma,
                TotalCapital = totalCapital,
                TotalInteres = totalInteres,
                TotalGasto = totalGasto,
                TotalIva = totalIva,
                TotalPagado = totalPagado,
                CostoTotalCredito = Math.Round(totalPagado - request.Monto, 2),
                TeaReal = teaReal,
                TeaMaximaLegal = teaMaximaLegal,
                SegmentoLegal = segmentoLegal,
                CumpleLeyUsura = cumpleLeyUsura
            };
        }

        private async Task<(decimal? TeaMaximaLegal, string? SegmentoLegal, bool CumpleLeyUsura)> ObtenerEvaluacionUsuraAsync(decimal montoCredito, DateTime fechaCredito, decimal teaReal)
        {
            var fechaConsulta = DateOnly.FromDateTime(fechaCredito);

            var salarioMinimo = await _context.SalarioMinimoVigentes
                .Where(s => s.BEstado && fechaConsulta >= s.DFecInicio && (s.DFecFin == null || fechaConsulta <= s.DFecFin))
                .OrderByDescending(s => s.DFecInicio)
                .FirstOrDefaultAsync();

            if (salarioMinimo is null || salarioMinimo.NMontoMensual <= 0)
                return (null, null, false);

            decimal montoEnSalarios = montoCredito / salarioMinimo.NMontoMensual;

            var segmento = await _context.CatSegmentoUsuras
                .Where(s => s.BEstado && montoEnSalarios > s.NMultSmmin && (s.NMultSmmax == null || montoEnSalarios <= s.NMultSmmax))
                .FirstOrDefaultAsync();

            TasaMaximaBcr? tasaMaxima = null;

            if (segmento is null)
            {
                tasaMaxima = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && fechaConsulta >= t.DFecInicio && fechaConsulta <= t.DFecFin)
                    .OrderByDescending(t => t.NTasaMaxima)
                    .FirstOrDefaultAsync();
            }
            else
            {
                tasaMaxima = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && t.NCodSegmento == segmento.NCodSegmento && fechaConsulta >= t.DFecInicio && fechaConsulta <= t.DFecFin)
                    .FirstOrDefaultAsync();
            }

            if (tasaMaxima is null)
                return (null, segmento?.CDescripcion, false);

            return (tasaMaxima.NTasaMaxima, segmento?.CDescripcion ?? "Segmento general", teaReal <= tasaMaxima.NTasaMaxima);
        }
    }
}
