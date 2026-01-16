
using CrediAvanzaAPI.Mappings;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class CalendarioService : ICalendarioService
    {
        private readonly DbNegocioContext _context;
        private readonly IGastoService _gastoService;
        private readonly IFeriadoService _feriadoService;

        public CalendarioService(DbNegocioContext context, IGastoService gastoService, IFeriadoService feriadoService)
        {
            _context = context;
            _gastoService = gastoService;
            _feriadoService = feriadoService;
        }
        public async Task<int> GenerarCalendarioAsync(int nCodCred)
        {
            //var credito = await _context.Creditos.FirstOrDefaultAsync(x => x.NCodCred == nCodCred) ?? throw new Exception("Crédito no existe");
            //var creditoRequest = credito.ToCreditoRequest();

            //decimal nGastoPorCuota = await _gastoService.ObtenerGastoAsync(creditoRequest);
            //var oFeriados = await _feriadoService.ObtenerFeriadosAsync(fechaDesembolso,creditoRequest.nCodAge);

            //// Luego validar:
            //bool esFeriado = oFeriados.Contains(fechaCuota);


            return 1;

        }
    }
}
