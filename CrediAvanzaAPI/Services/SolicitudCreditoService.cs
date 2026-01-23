using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using System;

namespace CrediAvanzaAPI.Services
{
    public class SolicitudCreditoService : ISolicitudCreditoService
    {
        private readonly DbNegocioContext _context;
        private readonly ErrorLogger _errorLogger;

        public SolicitudCreditoService(DbNegocioContext context, ErrorLogger errorLogger)
        {
            _context = context;
            _errorLogger = errorLogger;
        }

        public async Task<int> CrearSolicitudAsync(List<FotoId> fotoId, List<FotoDocumentacion> fotoDocumentacion, 
            List<FotoNegocio> fotoNegocio, Persona persona, Conyuge conyuge,
            Fiador fiador, Garantium garantium,
            Negocio negocio, List<Compra> compra, List<Venta> venta, Credito credito)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Personas.AddAsync(persona);
                await _context.Conyuges.AddAsync(conyuge); 
                await _context.Fiadors.AddAsync(fiador);
                await _context.Garantia.AddAsync(garantium);
                await _context.Negocios.AddAsync(negocio);
                var documentacion = new Documentacion();
                await _context.Documentacions.AddAsync(documentacion);
                await _context.SaveChangesAsync();

                compra.ForEach(c => c.IdNegocio = negocio.IdNegocio);
                venta.ForEach(v => v.IdNegocio = negocio.IdNegocio);
                fotoId.ForEach(f => f.IdPersona = persona.IdPersona);
                fotoDocumentacion.ForEach(f => f.IdDocumentacion = documentacion.IdDocumentacion);
                fotoNegocio.ForEach(f => f.IdNegocio = negocio.IdNegocio);

                await _context.Compras.AddRangeAsync(compra);
                await _context.Ventas.AddRangeAsync(venta);
                await _context.FotoIds.AddRangeAsync(fotoId);
                await _context.FotoDocumentacions.AddRangeAsync(fotoDocumentacion);
                await _context.FotoNegocios.AddRangeAsync(fotoNegocio);

                credito.IdPersona = persona.IdPersona;
                credito.IdConyuge = conyuge.IdConyuge;
                credito.IdDocumentacion = documentacion.IdDocumentacion;
                credito.IdGarantia = garantium.IdGarantia;
                credito.IdFiador = fiador.IdFiador;
                credito.IdNegocio = negocio.IdNegocio;

                await _context.Creditos.AddAsync(credito);
                int filasAfectadas = await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return filasAfectadas;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await _errorLogger.LogAsync(ex);
                throw;
            }
        }
    }
}