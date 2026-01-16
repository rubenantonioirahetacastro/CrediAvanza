using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;

namespace CrediAvanzaAPI.Mappings
{
    public static class CreditoExtensions
    {
        public static CreditoRequest ToCreditoRequest(this Credito credito)
        {
            if (credito == null)
                throw new ArgumentNullException(nameof(credito));

            return new CreditoRequest
            {
                nPrestamo = credito.NPrestamo,
                nProd = credito.NProd,
                nSubProd = credito.NSubProd,
                nTipoGasto = 1,   // Asumiendo 1 representa gasto de colecturia
                nPeriodo = credito.NPeriodo,
                nTipoCargo = 0, // Asumiendo 0 representa un tipo de cargo inicial 
                nCobroEnAgencia = credito.NCobroEnAgencia ?? 0,
                nCodCred = credito.NCodCred,
                nCodAge = credito.NCodAge
            };
        }
    }
}
