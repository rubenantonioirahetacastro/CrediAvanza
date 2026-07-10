namespace CrediAvanzaAPI.Helpers
{
    public class TasaUsuraException : Exception
    {
        public decimal TasaCredito { get; }
        public decimal TasaMaxima { get; }
        public int NCodSegmento { get; }

        public TasaUsuraException(decimal tasaCredito, decimal tasaMaxima, int nCodSegmento)
            : base($"La tasa del crédito ({tasaCredito:P2}) excede el máximo legal ({tasaMaxima:P2}) para el segmento {nCodSegmento}.")
        {
            TasaCredito = tasaCredito;
            TasaMaxima = tasaMaxima;
            NCodSegmento = nCodSegmento;
        }
    }
}