using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;

namespace CrediAvanzaAPI.Services
{
    public interface ISolicitudCreditoService
    {
        Task<int> CrearSolicitudAsync(
             List<FotoId> fotoIds,
             List<FotoDocumentacion> fotoDocumentacions,
             List<FotoNegocio> fotoNegocios,
         List<GarantiaConFotosRequest> garantias,
             Persona persona,
             Conyuge? conyuge,
             Fiador fiador,
             Negocio negocio,
             List<Compra> compra,
             List<Venta> venta,
             Credito credito
         );
    }
}
