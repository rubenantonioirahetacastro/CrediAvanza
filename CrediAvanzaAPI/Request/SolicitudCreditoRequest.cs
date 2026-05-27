using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Request
{
    public class GarantiaConFotosRequest
    {
        public Garantium Garantia { get; set; } = null!;
        public List<FotoGarantium> Fotos { get; set; } = new();
    }

    public class SolicitudCreditoRequest
    {
        public List<FotoId> FotoIds { get; set; } = new();
        public List<FotoDocumentacion> FotoDocumentacions { get; set; } = new();
        public List<FotoNegocio> FotoNegocios { get; set; } = new();
        public List<FotoGarantium> FotoGarantiums { get; set; } = new();
        public Persona Persona { get; set; } = null!;
        public Conyuge? Conyuge { get; set; }
        public Documentacion? Documentacion { get; set; }
        public Fiador Fiador { get; set; } = null!;
        public Garantium Garantia { get; set; } = null!;
        public List<GarantiaConFotosRequest> Garantias { get; set; } = new();
        public Negocio Negocio { get; set; } = null!;
        public List<Compra> Compra { get; set; } = new();
        public List<Venta> Venta { get; set; } = new();
        public Credito Credito { get; set; } = null!;

    }
}

