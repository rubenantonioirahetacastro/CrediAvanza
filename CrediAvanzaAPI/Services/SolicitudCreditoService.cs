using CrediAvanzaAPI.Models;

namespace CrediAvanzaAPI.Services
{
    public class SolicitudCreditoService : ISolicitudCreditoService
    {
        private readonly DbNegocioContext _context;

        public SolicitudCreditoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<int> CrearSolicitudAsync(Foto foto, Persona persona, Conyuge conyuge,
            Documentacion documentacion,Fiador fiador, Garantium garantium,
            Negocio negocio, List<Compra> compra, List<Venta> venta, Credito credito)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Fotos.AddAsync(foto);
                await _context.SaveChangesAsync();

                persona.IdFotoDocumento = foto.IdFoto;
                await _context.Personas.AddAsync(persona);
                await _context.Conyuges.AddAsync(conyuge); 
                await _context.Documentacions.AddAsync(documentacion);
                await _context.Fiadors.AddAsync(fiador);
                await _context.Garantia.AddAsync(garantium);
                await _context.Negocios.AddAsync(negocio);
                await _context.SaveChangesAsync();

                foreach (var c in compra)
                {
                    c.IdNegocio = negocio.IdNegocio;
                }

                foreach (var v in venta)
                {
                    v.IdNegocio = negocio.IdNegocio;
                }

                await _context.Compras.AddRangeAsync(compra);
                await _context.Ventas.AddRangeAsync(venta);

                credito.IdPersona = persona.IdPersona;
                credito.IdConyuge = conyuge.IdConyuge;
                credito.IdDocumentacion = documentacion.IdDocumento;
                credito.IdGarantia = garantium.IdGarantia;
                credito.IdFiador = fiador.IdFiador;
                credito.IdNegocio = negocio.IdNegocio;

                await _context.Creditos.AddAsync(credito);
                int filasAfectadas = await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return filasAfectadas;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}