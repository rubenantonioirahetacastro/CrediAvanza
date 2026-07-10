using CrediAvanzaAPI.Models;
using System;

namespace CrediAvanzaAPI.Helpers
{
    public static class CalculadoraFinanciera
    {
        private const int SUBPROD_DIARIO = 1;
        private const int SUBPROD_SEMANAL = 2;
        private const int SUBPROD_QUINCENAL = 3;
        private const int SUBPROD_MENSUAL = 4;

        private static int DiasNominalesPeriodo(int nSubProd) => nSubProd switch
        {
            SUBPROD_DIARIO => 1,
            SUBPROD_SEMANAL => 7,
            SUBPROD_QUINCENAL => 15,
            SUBPROD_MENSUAL => 30,
            _ => throw new ArgumentException($"nSubProd {nSubProd} no está definido para Crédito Microempresa.")
        };

        private static decimal FactorPeriodo(decimal tasaInteresMensual, int diasPeriodo)
            => (tasaInteresMensual * 12.0m / 365m) / 100.0m * diasPeriodo;

        public static decimal CalcularCuotaFijaEstimada(decimal saldo, int plazo, int nSubProd, decimal tasaInteresMensual, decimal tasaIva = 0.13m)
        {
            int diasPeriodo = DiasNominalesPeriodo(nSubProd);
            decimal rAprox = FactorPeriodo(tasaInteresMensual, diasPeriodo);

            if (rAprox > 0)
            {
                double baseExponente = (double)(1m + rAprox);
                decimal factor = (decimal)Math.Pow(baseExponente, plazo);
                return Math.Round(saldo * (rAprox * factor) / (factor - 1m), 2);
            }
            return Math.Round(saldo / plazo, 2);
        }

        public static decimal CalcularInteresCompensatorio(decimal saldo, int nSubProd, decimal tasaInteresMensual, int diasTranscurridos, decimal tasaIva = 0.13m)
        {
            return Math.Round(CalcularInteresCompensatorioBruto(saldo, nSubProd, tasaInteresMensual, diasTranscurridos), 2);
        }

        private static decimal CalcularInteresCompensatorioBruto(decimal saldo, int nSubProd, decimal tasaInteresMensual, int diasTranscurridos)
        {
            DiasNominalesPeriodo(nSubProd);
            decimal factorPeriodo = (tasaInteresMensual * 12.0m / 365m) / 100.0m;
            return saldo * factorPeriodo * diasTranscurridos;
        }

        public static decimal CalcularTeaPorTir(decimal montoCredito, List<CredCalendario> calendario, DateTime fechaDesembolso)
        {
            var flujos = calendario
                .OrderBy(c => c.DFecVenc)
                .Select(c => (dias: (c.DFecVenc.Date - fechaDesembolso.Date).Days, monto: c.NTotalCuota ?? 0m))
                .ToList();

            double tasaDiaria = 0.001;
            for (int iter = 0; iter < 100; iter++)
            {
                double van = -(double)montoCredito;
                double derivada = 0;
                foreach (var (dias, monto) in flujos)
                {
                    double factor = Math.Pow(1 + tasaDiaria, dias);
                    van += (double)monto / factor;
                    derivada += -dias * (double)monto / (factor * (1 + tasaDiaria));
                }
                if (Math.Abs(van) < 1e-9 || derivada == 0) break;
                tasaDiaria -= van / derivada;
                if (tasaDiaria <= -0.999) tasaDiaria = -0.5;
            }

            double tea = Math.Pow(1 + tasaDiaria, 360) - 1;
            return Math.Round((decimal)tea, 6);
        }

        public static List<CredCalendario> GenerarDetalleCuotasSimulado(
            decimal nCapital,
            int nPlazo,
            int nSubProd,
            decimal nTasaCom,
            DateTime fechaInicio,
            IEnumerable<DateTime>? oFeriados,
            decimal cuotaFijaEstimada,
            decimal tasaIva = 0.13m,
            decimal nGastoPorCuota = 0m,
            int nCodCalen = 0,
            int nCodAge = 0,
            int nCodCred = 0)
        {
            var feriadosSet = new HashSet<DateTime>((oFeriados ?? Array.Empty<DateTime>()).Select(f => f.Date));
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

            decimal cuotaReal = ResolverCuotaFijaExacta(nCapital, nSubProd, nTasaCom, diasPorCuota, tasaIva, nGastoPorCuota);
            var resultado = new List<CredCalendario>(nPlazo);
            decimal saldoIteracion = nCapital;

            for (int i = 1; i <= nPlazo; i++)
            {
                int diasTranscurridos = diasPorCuota[i - 1];
                DateTime fechaIteracion = fechas[i - 1];
                decimal interesCompensatorioBruto = CalcularInteresCompensatorioBruto(saldoIteracion, nSubProd, nTasaCom, diasTranscurridos);
                decimal interesCompensatorio = Math.Round(interesCompensatorioBruto, 2);

                decimal capitalCuota = (cuotaReal - interesCompensatorioBruto - nGastoPorCuota) / (1m + tasaIva);
                if (capitalCuota < 0)
                    capitalCuota = 0;

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
                if (saldoIteracion < 0)
                    saldoIteracion = 0;
            }

            return resultado;
        }

        private static decimal ResolverCuotaFijaExacta(decimal capital, int nSubProd, decimal tasaInteresMensual, IReadOnlyList<int> diasPorCuota, decimal tasaIva, decimal gastoPorCuota)
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
                    if (saldo <= 0) return saldo;
                }
                return saldo;
            }

            while (FinalSaldo(high) > 0m)
            {
                high *= 2m;
                if (high > capital * 100m)
                    break;
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
    }
}
