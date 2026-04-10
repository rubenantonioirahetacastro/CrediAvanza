using System;

namespace CrediAvanzaAPI.Helpers
{
    public static class CalculadoraFinanciera
    {
        public static decimal CalcularCuotaFijaEstimada(decimal saldo, int plazo, int nSubProd, decimal tasaInteresMensual, decimal tasaIva = 0.13m)
        {
            decimal rDiarioEstimacion = (tasaInteresMensual * 12.0m / 360m) / 100.0m;
            decimal rAprox;

            switch (nSubProd)
            {
                case 1:
                case 4: // Mensual
                    rAprox = (tasaInteresMensual * 12.0m / 360m) / 100.0m * 30m;
                    break;
                case 5: // Catorcenal
                    rAprox = rDiarioEstimacion * 14m;
                    break;
                case 2: // Diario
                    rAprox = (tasaInteresMensual / (1m + tasaIva) / 100m * 12.0m / 360m) * 1m;
                    break;
                default:
                    rAprox = rDiarioEstimacion * 30m;
                    break;
            }

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
            if (nSubProd == 5) // Catorcenal
            {
                decimal factorDiario = (tasaInteresMensual * 12.0m / 360m) / 100.0m;
                return Math.Round(saldo * factorDiario * diasTranscurridos, 2);
            }
            else if (nSubProd == 4 || nSubProd == 1) // Mensual
            {
                decimal factorDiarioMensual = (tasaInteresMensual * 12.0m / 360m) / 100.0m;
                return Math.Round(saldo * factorDiarioMensual * 30m, 2);
            }
            else if (nSubProd == 2) // Diario
            {
                decimal tasaSinIgv = tasaInteresMensual / (1m + tasaIva);
                decimal factorDiario = (tasaSinIgv / 100.0m * 12.0m / 360m);
                return Math.Round(saldo * factorDiario * diasTranscurridos, 2);
            }
            else
            {
                return Math.Round(saldo * (tasaInteresMensual / 100m), 2);
            }
        }
    }
}
