using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Services
{
    public class FeriadoService(DbNegocioContext context) : IFeriadoService
    {
        public async Task<List<DateTime>> ObtenerFeriadosAsync(DateTime fechaDesembolso, int codigoAgencia)
        {
            var feriados = await(
             from cf in context.CredFeriados
             join cfa in context.CredFeriadoAges
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
