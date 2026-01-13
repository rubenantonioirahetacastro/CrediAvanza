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

        public async Task<int> CrearSolicitudAsync(Foto foto, Persona persona, Compra compra, Documentacion documentacion,
             Fiador fiador, Garantium garantium, Venta venta, Credito credito)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Fotos.AddAsync(foto);
                await _context.Personas.AddAsync(persona);
                await _context.Compras.AddAsync(compra);
                await _context.Documentacions.AddAsync(documentacion);
                await _context.Fiadors.AddAsync(fiador);
                await _context.Garantia.AddAsync(garantium);
                await _context.Ventas.AddAsync(venta);

                await _context.SaveChangesAsync();

                persona.IdFotoDocumento = foto.IdFoto;

                credito.IdPersona = persona.IdPersona;
                credito.IdCompra = compra.IdCompra;
                credito.IdVenta = venta.IdVenta;
                credito.IdDocumentacion = documentacion.IdDocumento;
                credito.IdGarantia = garantium.IdGarantia;
                credito.IdFiador = fiador.IdFiador;

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