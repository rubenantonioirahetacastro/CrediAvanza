using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class CatalogoCodigoService : ICatalogoCodigoService
    {
        private readonly DbNegocioContext _context;
        private readonly DbSet<CatalogoCodigo> _dbSet;
        public CatalogoCodigoService(DbNegocioContext context)
        {
            _context = context;
            _dbSet = _context.Set<CatalogoCodigo>();
        }

        public async Task<int> AddCatalogo(CatalogoCodigo catalogo)
        {
            if (catalogo == null)
                return 0;

            await _dbSet.AddAsync(catalogo);
            return await _context.SaveChangesAsync();
        }


        public async Task<List<CatalogoCodigo>> AllCatalogos()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<int> DeleteCatalogo(int id)
        {
            var catalogo = await _context.CatalogoCodigos.FindAsync(id);
            if (catalogo == null)
            {
                return 0;
            }
            _context.CatalogoCodigos.Remove(catalogo);
            return await _context.SaveChangesAsync();
        }

        public async Task<CatalogoCodigo?> GetCatalogoById(int id)
        {
            return await _context.CatalogoCodigos.FindAsync(id);
        }

        public async Task<int> UpdateCatalogo(CatalogoCodigo catalogo)
        {
            var existe = await _dbSet.AnyAsync(x => x.NCodigo == catalogo.NCodigo);
            if (!existe)
                return 0;

            _dbSet.Update(catalogo);
            return await _context.SaveChangesAsync();
        }

    }
}
