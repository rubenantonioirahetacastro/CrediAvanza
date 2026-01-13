using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Request
{
    public class SolicitudCreditoRequest
    {
        public Foto Foto { get; set; }
        public Persona Persona { get; set; }
        public Compra Compra { get; set; }
        public Documentacion Documentacion { get; set; }
        public Fiador Fiador { get; set; }
        public Garantium Garantia { get; set; }
        public Venta Venta { get; set; }
        public Credito Credito { get; set; }
    }
}