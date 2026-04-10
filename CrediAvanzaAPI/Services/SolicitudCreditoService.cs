using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using System;

namespace CrediAvanzaAPI.Services
{
    public class SolicitudCreditoService(DbNegocioContext context, ErrorLogger errorLogger) : ISolicitudCreditoService
    {
        public async Task<int> CrearSolicitudAsync(List<FotoId> fotoId, List<FotoDocumentacion> fotoDocumentacion, 
            List<FotoNegocio> fotoNegocio, Persona persona, Conyuge conyuge,
            Fiador fiador, Garantium garantium,
            Negocio negocio, List<Compra> compra, List<Venta> venta, Credito credito)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                await context.Personas.AddAsync(persona);
                await context.Conyuges.AddAsync(conyuge); 
                await context.Fiadors.AddAsync(fiador);
                await context.Garantia.AddAsync(garantium);
                await context.Negocios.AddAsync(negocio);
                var documentacion = new Documentacion();
                await context.Documentacions.AddAsync(documentacion);
                await context.SaveChangesAsync();

                compra.ForEach(c => c.IdNegocio = negocio.IdNegocio);
                venta.ForEach(v => v.IdNegocio = negocio.IdNegocio);
                fotoId.ForEach(f => f.IdPersona = persona.IdPersona);
                fotoDocumentacion.ForEach(f => f.IdDocumentacion = documentacion.IdDocumentacion);
                fotoNegocio.ForEach(f => f.IdNegocio = negocio.IdNegocio);

                await context.Compras.AddRangeAsync(compra);
                await context.Ventas.AddRangeAsync(venta);
                await context.FotoIds.AddRangeAsync(fotoId);
                await context.FotoDocumentacions.AddRangeAsync(fotoDocumentacion);
                await context.FotoNegocios.AddRangeAsync(fotoNegocio);

                credito.IdPersona = persona.IdPersona;
                credito.IdConyuge = conyuge.IdConyuge;
                credito.IdDocumentacion = documentacion.IdDocumentacion;
                credito.IdGarantia = garantium.IdGarantia;
                credito.IdFiador = fiador.IdFiador;
                credito.IdNegocio = negocio.IdNegocio;

                await context.Creditos.AddAsync(credito);
                int filasAfectadas = await context.SaveChangesAsync();

                await tx.CommitAsync();
                return filasAfectadas;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                await errorLogger.LogAsync(ex);
                throw;
            }
        }
    }
}