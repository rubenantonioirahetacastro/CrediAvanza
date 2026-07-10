namespace CrediAvanzaAPI.Request
{
    public class SimularCalendarioRequest
    {
        public int NProd { get; set; }

        public int NSubProd { get; set; }

        public int NPlazo { get; set; }

        public decimal Monto { get; set; }

        public DateTime? FechaInicio { get; set; }

        public decimal? TasaOverride { get; set; }

        public decimal? GastoOverride { get; set; }
    }
}
