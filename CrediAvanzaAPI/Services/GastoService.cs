using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class GastoService : IGastoService
    {
        private readonly DbNegocioContext _context;

        public GastoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObtenerGastoAsync(CreditoRequest request)
        {
            // Implementa tu lógica de cálculo de gastos aquí
            // Este es un ejemplo básico, ajusta según tu lógica de negocio
            
            var gasto = await _context.CredGastos
                .Where(g => g.NProd == request.nProd 
                    && g.NSubProd == request.nSubProd
                    && g.NTipoGasto == request.nTipoGasto)
                .FirstOrDefaultAsync();

            return gasto?.NValor ?? 0m;
        }
    }
}
