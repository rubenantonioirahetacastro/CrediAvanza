using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ISolicitudCreditoService
    {
    //    public async Task<int> CrearSolicitudAsync(Foto foto, Persona persona, Conyuge conyuge,
    //Documentacion documentacion, Fiador fiador, Garantium garantium,
    //Negocio negocio, List<Compra> compra, List<Venta> venta, Credito credito)

        Task<int> CrearSolicitudAsync(
             List<Foto> foto,
             Persona persona,
             Conyuge conyuge,
             Documentacion documentacion,
             Fiador fiador,
             Garantium garantia,
             Negocio negocio,
             List<Compra> compra,
             List<Venta> venta,
             Credito credito
         );
    }
}
