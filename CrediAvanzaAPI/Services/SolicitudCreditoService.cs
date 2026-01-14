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

        public async Task<int> CrearSolicitudAsync(Foto foto, Persona persona, Compra compra, Conyuge conyuge, Documentacion documentacion,
             Fiador fiador, Garantium garantium, Venta venta, Credito credito, Negocio negocio)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Fotos.AddAsync(foto);

                persona.IdFotoDocumento = foto.IdFoto;
                await _context.Personas.AddAsync(persona);
                await _context.Compras.AddAsync(compra);
                await _context.Conyuges.AddAsync(conyuge); 
                await _context.Documentacions.AddAsync(documentacion);
                await _context.Fiadors.AddAsync(fiador);
                await _context.Garantia.AddAsync(garantium);
                await _context.Ventas.AddAsync(venta);
                await _context.Negocios.AddAsync(negocio);

                await _context.SaveChangesAsync();

                credito.IdPersona = persona.IdPersona;
                credito.IdCompra = compra.IdCompra;
                credito.IdConyuge = conyuge.IdConyuge;
                credito.IdVenta = venta.IdVenta;
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