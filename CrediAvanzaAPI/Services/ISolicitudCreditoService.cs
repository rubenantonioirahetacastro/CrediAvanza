using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ISolicitudCreditoService
    {
        Task<int> CrearSolicitudAsync(
             List<FotoId> fotoIds,
             List<FotoDocumentacion> fotoDocumentacions,
             List<FotoNegocio> fotoNegocios,
             Persona persona,
             Conyuge conyuge,
             Fiador fiador,
             Garantium garantia,
             Negocio negocio,
             List<Compra> compra,
             List<Venta> venta,
             Credito credito
         );
    }
}
