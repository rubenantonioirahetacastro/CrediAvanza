using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class SegmentoUsuraService : ISegmentoUsuraService
    {
        private readonly DbNegocioContext _context;
        private readonly ILogger<SegmentoUsuraService> _logger;

        public SegmentoUsuraService(DbNegocioContext context, ILogger<SegmentoUsuraService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ValidarTasaAsync(decimal montoCredito, List<CredCalendario> calendario, DateTime fechaCredito)
        {
            var fechaCredDate = DateOnly.FromDateTime(fechaCredito); // <-- conversion una sola vez

            var salarioMin = await _context.SalarioMinimoVigentes
                .Where(s => s.BEstado && fechaCredDate >= s.DFecInicio && (s.DFecFin == null || fechaCredDate <= s.DFecFin))
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException($"No hay salario mínimo vigente para {fechaCredito:yyyy-MM-dd}.");

            decimal montoEnSM = montoCredito / salarioMin.NMontoMensual;

            var segmento = await _context.CatSegmentoUsuras
                .Where(s => s.BEstado && montoEnSM > s.NMultSmmin && (s.NMultSmmax == null || montoEnSM <= s.NMultSmmax))
                .FirstOrDefaultAsync();

            decimal tasaMaximaAplicable;
            int codSegmento;

            if (segmento is null)
            {
                tasaMaximaAplicable = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && fechaCredDate >= t.DFecInicio && fechaCredDate <= t.DFecFin)
                    .MaxAsync(t => t.NTasaMaxima);
                codSegmento = 0;
                _logger.LogWarning("Monto {Monto} sin segmento definido. Aplicando tope general Art. 7.", montoCredito);
            }
            else
            {
                var tasaMaxima = await _context.TasaMaximaBcrs
                    .Where(t => t.BEstado && t.NCodSegmento == segmento.NCodSegmento
                        && fechaCredDate >= t.DFecInicio && fechaCredDate <= t.DFecFin)
                    .FirstOrDefaultAsync()
                    ?? throw new InvalidOperationException($"No hay tasa máxima BCR para segmento {segmento.NCodSegmento}.");
                tasaMaximaAplicable = tasaMaxima.NTasaMaxima;
                codSegmento = segmento.NCodSegmento;
            }

            decimal teaReal = CalculadoraFinanciera.CalcularTeaPorTir(montoCredito, calendario, fechaCredito);

            _logger.LogInformation("Validación usura: monto={Monto} segmento={Segmento} TEA={TEA:P4} TEAmax={TEAmax:P4}",
                montoCredito, codSegmento, teaReal, tasaMaximaAplicable);

            if (teaReal > tasaMaximaAplicable)
                throw new TasaUsuraException(teaReal, tasaMaximaAplicable, codSegmento);
        }
    }
}
