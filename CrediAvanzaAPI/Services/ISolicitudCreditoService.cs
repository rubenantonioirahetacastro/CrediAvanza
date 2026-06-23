using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ISolicitudCreditoService
    {
        Task<int> CrearSolicitudAsync(
             List<FotoId>? fotoIds,
             List<FotoDocumentacion>? fotoDocumentacions,
             List<GarantiaFoto>? fotoGarantias,
             List<FotoNegocio>? fotoNegocios,
             Persona persona,
             Conyuge? conyuge,
             Fiador? fiador,
             Negocio? negocio,
             CapacidadPago? capacidadPago,
             List<Compra>? compra,
             List<Venta>? venta,
             Credito credito
         );
    }
}
