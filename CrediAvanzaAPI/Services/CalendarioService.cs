using CrediAvanzaAPI.Mappings;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

using CrediAvanzaAPI.Helpers;

namespace CrediAvanzaAPI.Services
{
    public class CalendarioService : ICalendarioService
    {
        private readonly DbNegocioContext _context;
        private readonly IGastoService _gastoService;
        private readonly IFeriadoService _feriadoService;

        public CalendarioService(DbNegocioContext context, IGastoService gastoService, IFeriadoService feriadoService)
        {
            _context = context;
            _gastoService = gastoService;
            _feriadoService = feriadoService;
        }

        public async Task<List<CredCalendario>> ProyectarCalendarioAsync(decimal nCapital, int nPlazo, int nSubProd, int nCodAge)
        {
            var lineaCredito = await _context.CredLineaCreditos
                .FirstOrDefaultAsync(l => l.NSubProd == nSubProd && 
                                          nCapital >= l.NMontoMin && nCapital <= l.NMontoMax &&
                                          nPlazo >= l.NPlazoMin && nPlazo <= l.NPlazoMax &&
                                          l.BEstado)
                ?? throw new Exception("No existe una línea de crédito configurada para el subproducto, monto y plazo proporcionados.");

            decimal nTasaCom = lineaCredito.NTasaCom;

            DateTime fechaInicio = DateTime.Today;
            var oFeriados = await _feriadoService.ObtenerFeriadosAsync(fechaInicio, nCodAge);

            decimal tasaIva = 0.13m; // Tasa de IVA
            decimal cuotaFijaEstimada = CalculadoraFinanciera.CalcularCuotaFijaEstimada(nCapital, nPlazo, nSubProd, nTasaCom, tasaIva);

            return GenerarDetalleCuotas(nCapital, nPlazo, nSubProd, nTasaCom, fechaInicio, oFeriados, cuotaFijaEstimada, tasaIva, 0);
        }

        public async Task<List<CredCalendario>> GenerarCalendarioAsync(int nCodAge, int nCodCred)
        {
            var credito = await _context.Creditos
                .FirstOrDefaultAsync(x => x.NCodAge == nCodAge && x.NCodCred == nCodCred) 
                ?? throw new Exception("Crédito no existe");
            var creditoRequest = credito.ToCreditoRequest();

            // Obtener condiciones de calendario para el crédito
            var calendarioCond = await (
                from cc in _context.CredCalendConds
                join c in _context.Creditos on cc.NNroCalen equals c.IdCalendario
                where c.NCodAge == nCodAge && c.NCodCred == nCodCred
                select cc
            ).FirstOrDefaultAsync() ?? throw new Exception("Condiciones de calendario no encontradas");

            // Obtener la tasa de interés desde la Línea de Crédito asociada
            var lineaCredito = await _context.CredLineaCreditos
                .FirstOrDefaultAsync(l => l.NCodLinea == credito.NCodLinea)
                ?? throw new Exception("Línea de crédito no encontrada para obtener la tasa.");

            decimal tasaInteresMensual = lineaCredito.NTasaCom; 

            var oFeriados = await _feriadoService.ObtenerFeriadosAsync(credito.DFecVig, nCodAge);
            decimal nGastoPorCuota = await _gastoService.ObtenerGastoAsync(creditoRequest);

            int totalCuotas = calendarioCond.NCuotas;
            DateTime fecha = credito.DFecVig;
            decimal saldo = credito.NSaldoK; // C: Monto del préstamo (o saldo actual)

            decimal tasaIva = 0.13m; // Tasa de IVA (ej. 13%)
            int n = totalCuotas;

            // Delegar cálculo central a helper financiero para evitar duplicidad
            decimal cuotaFijaEstimada = CalculadoraFinanciera.CalcularCuotaFijaEstimada(saldo, n, credito.NSubProd, tasaInteresMensual, tasaIva);

            return GenerarDetalleCuotas(saldo, n, credito.NSubProd, tasaInteresMensual, fecha, oFeriados, cuotaFijaEstimada, tasaIva, calendarioCond.NNroCalen);
        }

        private List<CredCalendario> GenerarDetalleCuotas(
            decimal nCapital, int nPlazo, int nSubProd, decimal nTasaCom, 
            DateTime fechaInicio, IEnumerable<DateTime> oFeriados, decimal cuotaFijaEstimada, 
            decimal tasaIva, int nCodCalen)
        {
            var resultado = new List<CredCalendario>();
            decimal saldoIteracion = nCapital;
            DateTime fechaAnterior = fechaInicio;
            DateTime fechaIteracion = fechaInicio;

            for (int i = 1; i <= nPlazo; i++)
            {
                switch (nSubProd)
                {
                    case 1:
                    case 4: 
                        fechaIteracion = fechaInicio.AddMonths(i); 
                        break;
                    case 5: // Catorcenal
                        fechaIteracion = fechaInicio.AddDays(14 * i); 
                        break;
                    case 2: // Diario
                        fechaIteracion = fechaAnterior.AddDays(1); 
                        break;
                    default: 
                        fechaIteracion = fechaInicio.AddMonths(i); 
                        break;
                }

                while (fechaIteracion.DayOfWeek == DayOfWeek.Saturday || fechaIteracion.DayOfWeek == DayOfWeek.Sunday || oFeriados.Contains(fechaIteracion.Date))
                {
                    fechaIteracion = fechaIteracion.AddDays(1);
                }

                int diasTranscurridos = (fechaIteracion.Date - fechaAnterior.Date).Days;

                decimal interesCompensatorio = CalculadoraFinanciera.CalcularInteresCompensatorio(
                    saldoIteracion, 
                    nSubProd, 
                    nTasaCom, 
                    diasTranscurridos, 
                    tasaIva);

                decimal iva = Math.Round(interesCompensatorio * tasaIva, 2);
                decimal capitalCuota = 0m;

                if (i < nPlazo)
                {
                    capitalCuota = cuotaFijaEstimada - interesCompensatorio - iva;
                    if (capitalCuota < 0) capitalCuota = 0;
                }
                else
                {
                    capitalCuota = saldoIteracion;
                }

                var item = new CredCalendario
                {
                    NNroCuota = i,
                    DFecVenc = fechaIteracion,
                    NCapital = capitalCuota,
                    NIntComp = interesCompensatorio, 
                    NIntMor = 0m,
                    NIgv = iva, 
                    nTotalCuota = Math.Round(capitalCuota + interesCompensatorio + iva, 2),
                    NCapPag = 0m,
                    NIntPag = 0m,
                    NIntMorPag = 0m,
                    NIgvPag = 0m,
                    NEstado = 0,
                    NNroCalen = nCodCalen    
                };

                resultado.Add(item);
                saldoIteracion -= capitalCuota;
                if (saldoIteracion < 0) saldoIteracion = 0;
                
                fechaAnterior = fechaIteracion;
            }

            return resultado;
        }
    }
}
