using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public class AgenciaService : IAgenciaService
    {
        private readonly DbNegocioContext _context;

        public AgenciaService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<Agencia>> AllAgencias()
        {
            return await _context.Agencias.ToListAsync();
        }

        public async Task<Agencia?> GetAgenciaById(int id)
        {
            return await _context.Agencias.FindAsync(id);
        }

        public async Task<bool> AddAgencia(Agencia agencia)
        {
            if (agencia == null)
                throw new ArgumentNullException(nameof(agencia));

            await _context.Agencias.AddAsync(agencia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAgencia(Agencia agencia)
        {
            if (agencia == null)
                throw new ArgumentNullException(nameof(agencia));

            var existente = await _context.Agencias.FindAsync(agencia.NCodAge);
            if (existente == null)
                return false;

            existente.CNomAge = agencia.CNomAge;
            existente.CDirecAge = agencia.CDirecAge;
            existente.CTelefAge = agencia.CTelefAge;
            existente.CCorreoElectronico = agencia.CCorreoElectronico;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAgencia(int id)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null)
                return false;

            _context.Agencias.Remove(agencia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}