namespace CrediAvanzaAPI.Services
{
    public interface ICalendarioService
    {
        Task<int> GenerarCalendarioAsync(int idCredito);
    }
}
