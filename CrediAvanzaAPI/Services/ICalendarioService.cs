using System.Collections.Generic;
using System.Threading.Tasks;
using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public interface ICalendarioService
    {
        Task<List<CredCalendario>> GenerarCalendarioAsync(int nCodAge, int nCodCred);
        Task<List<CredCalendario>> ProyectarCalendarioAsync(decimal nCapital, int nPlazo, int nSubProd, int nCodAge);
    }
}
