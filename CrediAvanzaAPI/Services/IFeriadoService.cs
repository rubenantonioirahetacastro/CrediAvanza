namespace CrediAvanzaAPI.Services
{
    public interface IFeriadoService
    {
        Task<List<DateTime>> ObtenerFeriadosAsync(DateTime fechaDesembolso, int codigoAgencia);
    }
}
