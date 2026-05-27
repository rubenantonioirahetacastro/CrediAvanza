using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using System;

namespace CrediAvanzaAPI.Services
{
    public class SolicitudCreditoService(DbNegocioContext context, ErrorLogger errorLogger) : ISolicitudCreditoService
    {
        public async Task<int> CrearSolicitudAsync(List<FotoId> fotoId, List<FotoDocumentacion> fotoDocumentacion, 
            List<FotoNegocio> fotoNegocio, List<GarantiaConFotosRequest> garantias, Persona persona, Conyuge? conyuge,
            Fiador fiador,
            Negocio negocio, List<Compra> compra, List<Venta> venta, Credito credito)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                await context.Personas.AddAsync(persona);

                if (conyuge != null)
                    await context.Conyuges.AddAsync(conyuge);

                await context.Fiadors.AddAsync(fiador);
                await context.Negocios.AddAsync(negocio);
                var documentacion = new Documentacion();
                await context.Documentacions.AddAsync(documentacion);

                if (garantias == null || garantias.Count == 0)
                    throw new Exception("Debe enviar al menos una garantía.");

                foreach (var g in garantias)
                {
                    if (g.Garantia == null)
                        throw new Exception("Garantía inválida.");

                    if (g.Fotos == null)
                        g.Fotos = new List<FotoGarantium>();

                    // nIdFotoGarantia NO permite null en BD, garantizamos al menos 1 foto placeholder
                    if (g.Fotos.Count == 0)
                    {
                        g.Fotos.Add(new FotoGarantium
                        {
                            VFoto = null,
                            IdTipoGarantia = 0
                        });
                    }

                    await context.Garantia.AddAsync(g.Garantia);
                }

                await context.SaveChangesAsync();

                compra.ForEach(c => c.IdNegocio = negocio.IdNegocio);
                venta.ForEach(v => v.IdNegocio = negocio.IdNegocio);
                fotoId.ForEach(f => f.IdPersona = persona.IdPersona);
                fotoDocumentacion.ForEach(f => f.IdDocumentacion = documentacion.IdDocumentacion);
                fotoNegocio.ForEach(f => f.IdNegocio = negocio.IdNegocio);

                foreach (var g in garantias)
                {
                    g.Fotos.ForEach(f => f.IdGarantia = g.Garantia.IdGarantia);
                }
               

                await context.Compras.AddRangeAsync(compra);
                await context.Ventas.AddRangeAsync(venta);
                await context.FotoIds.AddRangeAsync(fotoId);
                await context.FotoDocumentacions.AddRangeAsync(fotoDocumentacion);
                await context.FotoNegocios.AddRangeAsync(fotoNegocio);

                foreach (var g in garantias)
                {
                    await context.Set<FotoGarantium>().AddRangeAsync(g.Fotos);
                }

                // Guardar fotos para obtener IdFoto y poder asignar nIdFotoGarantia
                await context.SaveChangesAsync();

                foreach (var g in garantias)
                {
                    var fotoPrincipal = g.Fotos.OrderBy(f => f.IdFoto).FirstOrDefault();
                    if (fotoPrincipal == null)
                        throw new Exception("No se pudo generar foto principal de la garantía.");

                    g.Garantia.NIdFotoGarantia = fotoPrincipal.IdFoto;
                }

                credito.IdPersona = persona.IdPersona;

                if (conyuge != null)
                    credito.IdConyuge = conyuge.IdConyuge;

                credito.IdDocumentacion = documentacion.IdDocumentacion;
                credito.IdFiador = fiador.IdFiador;
                credito.IdNegocio = negocio.IdNegocio;

                // Mantener compatibilidad con el esquema actual del crédito: usa la primera garantía como principal
                credito.IdGarantia = garantias[0].Garantia.IdGarantia;

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