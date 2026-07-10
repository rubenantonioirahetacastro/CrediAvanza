using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Response;

namespace CrediAvanzaAPI.Services
{
    public interface ISimulacionCalendarioService
    {
        Task<SimularCalendarioResponse> SimularAsync(SimularCalendarioRequest request);
    }
}
