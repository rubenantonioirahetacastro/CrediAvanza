using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ISegmentoUsuraService
    {
        Task ValidarTasaAsync(decimal montoCredito, List<CredCalendario> calendario, DateTime fechaCredito);
    }
}
