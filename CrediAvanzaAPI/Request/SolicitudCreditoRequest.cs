using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Http;

namespace CrediAvanzaAPI.Request
{
    // Request DTO that uses per-photo request objects which may include uploaded files
    public class SolicitudCreditoRequest
    {
        public List<FotoIdRequest>? FotoIds { get; set; }
        public List<FotoDocumentacionRequest>? FotoDocumentacions { get; set; }
        public List<FotoNegocioRequest>? FotoNegocios { get; set; }
        public List<GarantiaFotoRequest>? GarantiaFotos { get; set; }
        public Persona Persona { get; set; }
        public Conyuge? Conyuge { get; set; }
        public Documentacion? Documentacion { get; set; }
        public Fiador? Fiador { get; set; }
        public Garantium? Garantia { get; set; }
        public Negocio? Negocio { get; set; }
        public CapacidadPago? CapacidadPago { get; set; }
        public List<Compra>? Compra { get; set; }
        public List<Venta>? Venta { get; set; }
        public Credito Credito { get; set; }
    }

    public class FotoIdRequest
    {
        public int IdFoto { get; set; }
        public string? VFoto { get; set; }
        public int NTipoFoto { get; set; }
        public int IdPersona { get; set; }
        public IFormFile? Archivo { get; set; }
    }

    public class FotoDocumentacionRequest
    {
        public int IdFoto { get; set; }
        public string? VFoto { get; set; }
        public int IdTipoDocumentacion { get; set; }
        public int IdDocumentacion { get; set; }
        public IFormFile? Archivo { get; set; }
    }

    public class FotoNegocioRequest
    {
        public int IdFoto { get; set; }
        public string? VFoto { get; set; }
        public int NTipoFoto { get; set; }
        public int IdNegocio { get; set; }
        public IFormFile? Archivo { get; set; }
    }

    public class GarantiaFotoRequest
    {
        public int IdFoto { get; set; }
        public string? VFoto { get; set; }
        public decimal NValor { get; set; }
        public int IdArticuloGarantia { get; set; }
        public int IdGarantia { get; set; }
        public IFormFile? Archivo { get; set; }
    }
}

