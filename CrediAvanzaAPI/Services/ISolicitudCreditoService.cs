using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ISolicitudCreditoService
    {
        Task<int> CrearSolicitudAsync(
             Foto foto,
             Persona persona,
             Compra compra, 
             Conyuge conyuge,
             Documentacion documentacion,
             Fiador fiador,
             Garantium garantia,
             Venta venta,
             Credito credito
         );
    }
}
