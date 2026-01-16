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
            var cobraEnAgencia = await _context.Creditos
                .Where(x => x.NCodCred == request.nCodCred)
                .Select(x => x.NCobroEnAgencia ?? 0)
                .FirstOrDefaultAsync();

            if (cobraEnAgencia == 1)
                return 0m;

            var gastoCambio = await _context.CredCambioGastos
                .Where(x => x.NCodCred == request.nCodCred)
                .Select(x => x.NMontoNuevo)
                .FirstOrDefaultAsync();

            if (gastoCambio > 0m)
                return gastoCambio;

            return await _context.CredGastos
                .Where(x =>
                    request.nPrestamo >= x.NRangoInicial &&
                    request.nPrestamo <= x.NRangoFinal &&
                    x.NProd == request.nProd &&
                    x.NSubProd == request.nSubProd &&
                    x.NTipoGasto == request.nTipoGasto &&
                    x.NPeriodo == request.nPeriodo &&
                    x.NTipoCargo == request.nTipoCargo
                )
                .Select(x => x.NValor)
                .FirstOrDefaultAsync();
        }


    }
}
