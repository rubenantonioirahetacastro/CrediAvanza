using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Request
{
    public class SolicitudCreditoRequest
    {
        public List<Foto> Foto { get; set; }
        public Persona Persona { get; set; }
        public Conyuge Conyuge { get; set; }
        public Documentacion Documentacion { get; set; }
        public Fiador Fiador { get; set; }
        public Garantium Garantia { get; set; }
        public Negocio Negocio { get; set; }
        public List<Compra> Compra { get; set; }
        public List<Venta> Venta { get; set; }
        public Credito Credito { get; set; }
      
    }
}

