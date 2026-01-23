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

        public async Task<bool> AddCatalogo(CatalogoCodigo catalogo)
        {
            if (catalogo == null)
                throw new ArgumentNullException(nameof(catalogo));

            var existe = await _context.CatalogoCodigos.AnyAsync(x =>
                x.NCodigo == catalogo.NCodigo &&
                x.NValor == catalogo.NValor);

            if (existe)
                throw new InvalidOperationException(
                    $"Ya existe el valor {catalogo.NValor} para el catálogo {catalogo.NCodigo}"
                );

            await _context.CatalogoCodigos.AddAsync(catalogo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CatalogoCodigo>> AllCatalogos()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<CatalogoCodigo?> GetByKeyAsync(int nCodigo, int nValor)
        {
            return await _context.CatalogoCodigos
                .FirstOrDefaultAsync(x => x.NCodigo == nCodigo && x.NValor == nValor);
        }

        public async Task<bool> DeleteCatalogo(int nCodigo, int nValor)
        {
            var entity = await GetByKeyAsync(nCodigo, nValor);

            if (entity == null)
                return false;

            _context.CatalogoCodigos.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CatalogoCodigo>> GetCatalogoById(int nCodigo)
        {
            return await _context.CatalogoCodigos
               .Where(x => x.NCodigo == nCodigo)
               .OrderBy(x => x.NValor)
               .ToListAsync();
        }


        public async Task<bool> UpdateCatalogo(CatalogoCodigo catalogo)
        {
            if (catalogo == null)
                throw new ArgumentNullException(nameof(catalogo));

            var existente = await _context.CatalogoCodigos
                .FirstOrDefaultAsync(x =>
                    x.NCodigo == catalogo.NCodigo &&
                    x.NValor == catalogo.NValor);

            if (existente == null)
                return false;

            //NO tocar la PK
            existente.CNomCod = catalogo.CNomCod;
            existente.NEstados = catalogo.NEstados;
            existente.NTipoCodigo = catalogo.NTipoCodigo;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
