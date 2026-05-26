using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;

namespace CrediAvanzaAPI.Services
{
    public interface IPagoService
    {
        Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request);
    }
}