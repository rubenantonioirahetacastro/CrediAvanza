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

            return GenerarDetalleCuotas(nCapital, nPlazo, nSubProd, nTasaCom, fechaInicio, oFeriados, cuotaFijaEstimada, tasaIva, 0m, nCodAge, 0, 0);
        }

        public async Task<List<CredCalendario>> GenerarCalendarioAsync(int nCodAge, int nCodCred)
        {
            var credito = await _context.Creditos
                .FirstOrDefaultAsync(x => x.NCodAge == nCodAge && x.NCodCred == nCodCred) 
                ?? throw new Exception("Crédito no existe");
            var creditoRequest = credito.ToCreditoRequest();

            // Obtener condiciones de calendario para el crédito
            var calendarioCond = await _context.CredCalendConds
                .FirstOrDefaultAsync(cc => cc.IdCredCalendCond == credito.IdCredCalendCond)
                    ?? throw new Exception("Condiciones de calendario no encontradas");

            calendarioCond.NNroCalen += 1;

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

            // Delegar cálculo central a helper financiero para evitar duplicidad
            decimal cuotaFijaEstimada = CalculadoraFinanciera.CalcularCuotaFijaEstimada(saldo, totalCuotas, credito.NSubProd, tasaInteresMensual, tasaIva);

            var calendario = GenerarDetalleCuotas(saldo, totalCuotas, credito.NSubProd, tasaInteresMensual, fecha, oFeriados, cuotaFijaEstimada, tasaIva, nGastoPorCuota, calendarioCond.NNroCalen, credito.NCodAge, credito.NCodCred);

            await _context.Set<CredCalendario>().AddRangeAsync(calendario);
            await _context.SaveChangesAsync();

            return calendario;
        }

        private List<CredCalendario> GenerarDetalleCuotas(
        decimal nCapital, int nPlazo, int nSubProd, decimal nTasaCom,
        DateTime fechaInicio, IEnumerable<DateTime> oFeriados, decimal cuotaFijaEstimada,
        decimal tasaIva, decimal nGastoPorCuota, int nCodCalen, int nCodAge, int nCodCred)
        {
            var feriadosSet = new HashSet<DateTime>(oFeriados.Select(f => f.Date));
            var fechas = new List<DateTime>(nPlazo);
            var diasPorCuota = new List<int>(nPlazo);

            DateTime fechaAnterior = fechaInicio;

            for (int i = 1; i <= nPlazo; i++)
            {
                DateTime fechaIteracion = nSubProd switch
                {
                    1 => fechaAnterior.AddDays(1),
                    2 => fechaInicio.AddDays(7 * i),
                    3 => fechaInicio.AddDays(15 * i),
                    4 => fechaInicio.AddMonths(i),
                    _ => throw new ArgumentException($"nSubProd {nSubProd} no está definido.")
                };

                while (fechaIteracion.DayOfWeek == DayOfWeek.Saturday
                    || fechaIteracion.DayOfWeek == DayOfWeek.Sunday
                    || feriadosSet.Contains(fechaIteracion.Date))
                {
                    fechaIteracion = fechaIteracion.AddDays(1);
                }

                fechas.Add(fechaIteracion);
                diasPorCuota.Add((fechaIteracion.Date - fechaAnterior.Date).Days);
                fechaAnterior = fechaIteracion;
            }

            decimal cuotaReal = ResolverCuotaFijaExacta(nCapital, nTasaCom, diasPorCuota, tasaIva, nGastoPorCuota);
            var resultado = new List<CredCalendario>(nPlazo);
            decimal saldoIteracion = nCapital;

            for (int i = 1; i <= nPlazo; i++)
            {
                int diasTranscurridos = diasPorCuota[i - 1];
                DateTime fechaIteracion = fechas[i - 1];
                decimal interesCompensatorioBruto = GetInteresCompensatorioBruto(saldoIteracion, nSubProd, nTasaCom, diasTranscurridos);
                decimal interesCompensatorio = Math.Round(interesCompensatorioBruto, 2);

                decimal capitalCuota = (cuotaReal - interesCompensatorioBruto - nGastoPorCuota) / (1m + tasaIva);
                if (capitalCuota < 0) capitalCuota = 0;
                capitalCuota = Math.Round(capitalCuota, 6);
                if (i == nPlazo)
                    capitalCuota = saldoIteracion;

                decimal ivaCapital = Math.Round(capitalCuota * tasaIva, 2);
                decimal totalCuota = Math.Round(capitalCuota + interesCompensatorio + nGastoPorCuota + ivaCapital, 2);

                resultado.Add(new CredCalendario
                {
                    NCodAge = nCodAge,
                    NCodCred = nCodCred,
                    NNroCalen = nCodCalen,
                    NNroCuota = i,
                    DFecVenc = fechaIteracion,
                    NCapital = Math.Round(capitalCuota, 2),
                    NIntComp = interesCompensatorio,
                    NIntMor = 0m,
                    Ngasto = nGastoPorCuota,
                    NIgv = ivaCapital,
                    NCapPag = 0m,
                    NIntPag = 0m,
                    NIntMorPag = 0m,
                    NIgvPag = 0m,
                    NTotalCuota = totalCuota,
                    NEstado = 0
                });

                saldoIteracion -= capitalCuota;
                if (saldoIteracion < 0) saldoIteracion = 0;
            }

            return resultado;
        }

        private static decimal ResolverCuotaFijaExacta(decimal capital, decimal tasaInteresMensual, IReadOnlyList<int> diasPorCuota, decimal tasaIva, decimal gastoPorCuota)
        {
            decimal low = 0m;
            decimal high = capital * 2m + 1m;

            decimal FinalSaldo(decimal cuota)
            {
                decimal saldo = capital;
                foreach (var dias in diasPorCuota)
                {
                    decimal interesBruto = saldo * ((tasaInteresMensual * 12.0m / 365m) / 100.0m) * dias;
                    decimal capitalCuota = (cuota - interesBruto - gastoPorCuota) / (1m + tasaIva);
                    if (capitalCuota < 0) capitalCuota = 0;
                    saldo -= capitalCuota;
                    if (saldo <= 0)
                        return saldo;
                }

                return saldo;
            }

            while (FinalSaldo(high) > 0m && high < capital * 100m)
            {
                high *= 2m;
            }

            for (int i = 0; i < 80; i++)
            {
                decimal mid = (low + high) / 2m;
                decimal saldoFinal = FinalSaldo(mid);
                if (Math.Abs(saldoFinal) < 0.000001m)
                    return mid;

                if (saldoFinal > 0m)
                    low = mid;
                else
                    high = mid;
            }

            return (low + high) / 2m;
        }

        private static decimal GetInteresCompensatorioBruto(decimal saldo, int nSubProd, decimal tasaInteresMensual, int diasTranscurridos)
        {
            _ = nSubProd;
            decimal factorPeriodo = (tasaInteresMensual * 12.0m / 365m) / 100.0m;
            return saldo * factorPeriodo * diasTranscurridos;
        }
    }
}
