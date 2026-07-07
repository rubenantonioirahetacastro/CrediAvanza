using System;

namespace CrediAvanzaAPI.Helpers
{
    public static class CalculadoraFinanciera
    {
        // Mapeo real según tabla credlineacredito (nProd = 1, Crédito Microempresa)
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

        // Todos los subproductos incluyen IVA en nTasaCom, por lo tanto todos
        // deben excluirlo antes de calcular el factor diario.
        private static decimal FactorDiario(decimal tasaInteresMensual, decimal tasaIva)
        {
            decimal tasaSinIva = tasaInteresMensual / (1m + tasaIva);
            return (tasaSinIva * 12.0m / 360m) / 100.0m;
        }

        public static decimal CalcularCuotaFijaEstimada(decimal saldo, int plazo, int nSubProd, decimal tasaInteresMensual, decimal tasaIva = 0.13m)
        {
            int diasPeriodo = DiasNominalesPeriodo(nSubProd);
            decimal factorDiario = FactorDiario(tasaInteresMensual, tasaIva);
            decimal rAprox = factorDiario * diasPeriodo;

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
            DiasNominalesPeriodo(nSubProd); // valida que el subproducto exista
            decimal factorDiario = FactorDiario(tasaInteresMensual, tasaIva);
            return Math.Round(saldo * factorDiario * diasTranscurridos, 2);
        }
    }
}
