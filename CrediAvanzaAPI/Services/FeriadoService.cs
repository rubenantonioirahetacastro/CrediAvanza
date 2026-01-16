using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class FeriadoService : IFeriadoService
    {
        private readonly DbNegocioContext _context;

        public FeriadoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> ObtenerFeriadosAsync(DateTime fechaDesembolso, int codigoAgencia)
        {
            var feriados = await(
             from cf in _context.CredFeriados
             join cfa in _context.CredFeriadoAges
                 on cf.NIdFeriado equals cfa.NIdFeriado
             where cf.BEstado == true
                   && cfa.NCodAge == codigoAgencia
                   && cf.DFecha >= fechaDesembolso.Date
             select cf.DFecha
             ).ToListAsync();
            return feriados;
        }
    }
}
