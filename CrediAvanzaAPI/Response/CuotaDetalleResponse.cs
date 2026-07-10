namespace CrediAvanzaAPI.Response
{
    public class CuotaDetalleResponse
    {
        public int NroCuota { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal Capital { get; set; }

        public decimal Interes { get; set; }

        public decimal Gasto { get; set; }

        public decimal Iva { get; set; }

        public decimal TotalCuota { get; set; }

        public decimal SaldoDespues { get; set; }
    }
}
