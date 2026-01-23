using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ICatalogoCodigoService
    {
        Task<List<CatalogoCodigo>> AllCatalogos();
        Task<bool> AddCatalogo(CatalogoCodigo catalogo);
        Task<bool> UpdateCatalogo(CatalogoCodigo catalogo);
        Task<bool> DeleteCatalogo(int nCodigo, int nValor);
        Task<List<CatalogoCodigo>> GetCatalogoById(int nCodigo);
    }
}
