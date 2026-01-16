using CrediAvanzaAPI.Request;

namespace CrediAvanzaAPI.Services
{
    public interface IGastoService
    {
        Task<decimal> ObtenerGastoAsync(CreditoRequest request);
    }
}
