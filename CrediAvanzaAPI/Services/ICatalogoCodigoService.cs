using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ICatalogoCodigoService
    {
        Task<List<CatalogoCodigo>> AllCatalogos();
        Task<int> AddCatalogo(CatalogoCodigo catalogo);
        Task<int> UpdateCatalogo(CatalogoCodigo catalogo);
        Task<int> DeleteCatalogo(int id);
        Task<CatalogoCodigo?> GetCatalogoById(int id);
    }
}
