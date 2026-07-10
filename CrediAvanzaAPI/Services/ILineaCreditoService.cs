using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public interface ILineaCreditoService
    {
        Task<CredLineaCredito> ResolverLineaCreditoAsync(int nSubProd, decimal montoSolicitado);
    }

    public class LineaCreditoService : ILineaCreditoService
    {
        private readonly DbNegocioContext _context;
        public LineaCreditoService(DbNegocioContext context) => _context = context;

        public async Task<CredLineaCredito> ResolverLineaCreditoAsync(int nSubProd, decimal montoSolicitado)
        {
            var linea = await _context.CredLineaCreditos
                .Where(l => l.NSubProd == nSubProd && l.BEstado
                    && montoSolicitado >= l.NMontoMin && montoSolicitado <= l.NMontoMax)
                .FirstOrDefaultAsync();

            if (linea is null)
                throw new InvalidOperationException($"No existe línea de crédito para subproducto {nSubProd} con monto ${montoSolicitado}.");

            return linea;
        }
    }
}
