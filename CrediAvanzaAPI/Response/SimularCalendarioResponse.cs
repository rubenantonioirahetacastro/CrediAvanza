using System.Collections.Generic;

namespace CrediAvanzaAPI.Response
{
    public class SimularCalendarioResponse
    {
        public string? LineaUsada { get; set; }

        public decimal TasaNominalMensual { get; set; }

        public decimal MontoSolicitado { get; set; }

        public int Plazo { get; set; }

        public List<CuotaDetalleResponse> Cronograma { get; set; } = new();

        public decimal TotalCapital { get; set; }

        public decimal TotalInteres { get; set; }

        public decimal TotalGasto { get; set; }

        public decimal TotalIva { get; set; }

        public decimal TotalPagado { get; set; }

        public decimal CostoTotalCredito { get; set; }

        public decimal TeaReal { get; set; }

        public decimal? TeaMaximaLegal { get; set; }

        public string? SegmentoLegal { get; set; }

        public bool CumpleLeyUsura { get; set; }
    }
}
