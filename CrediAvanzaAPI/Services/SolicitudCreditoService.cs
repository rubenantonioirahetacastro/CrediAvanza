using CrediAvanzaAPI.Helpers;
using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt.Net;

namespace CrediAvanzaAPI.Services
{
    public class SolicitudCreditoService(
        DbNegocioContext context,
        ErrorLogger errorLogger,
        IEmailService emailService,
        ICalendarioService calendarioService) : ISolicitudCreditoService
    {
        public async Task<int> CrearSolicitudAsync(
            List<FotoId>? fotoId,
            List<FotoDocumentacion>? fotoDocumentacion,
            List<GarantiaFoto>? fotoGarantia,
            List<FotoNegocio>? fotoNegocio,
            Persona persona,
            Conyuge? conyuge,
            Fiador? fiador,
            Negocio? negocio,
            CapacidadPago? capacidadPago,
            List<Compra>? compra,
            List<Venta>? venta,
            Credito credito)
        {
            await using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                var documentacion = new Documentacion();
                var garantia = new Garantium();

                await context.Personas.AddAsync(persona);

                if (conyuge != null) await context.Conyuges.AddAsync(conyuge);
                if (fiador != null) await context.Fiadors.AddAsync(fiador);
                if (negocio != null) await context.Negocios.AddAsync(negocio);

                await context.Documentacions.AddAsync(documentacion);
                await context.Garantia.AddAsync(garantia);
                if (capacidadPago != null) await context.CapacidadPagos.AddAsync(capacidadPago);

                await context.SaveChangesAsync();

                compra?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);
                venta?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);

                fotoId?.ForEach(x => x.IdPersona = persona.IdPersona);
                fotoDocumentacion?.ForEach(x => x.IdDocumentacion = documentacion.IdDocumentacion);
                fotoGarantia?.ForEach(x => x.IdGarantia = garantia.IdGarantia);
                fotoNegocio?.ForEach(x => x.IdNegocio = negocio!.IdNegocio);

                if (compra?.Any() == true) await context.Compras.AddRangeAsync(compra);
                if (venta?.Any() == true) await context.Ventas.AddRangeAsync(venta);
                if (fotoId?.Any() == true) await context.FotoIds.AddRangeAsync(fotoId);
                if (fotoDocumentacion?.Any() == true) await context.FotoDocumentacions.AddRangeAsync(fotoDocumentacion);
                if (fotoGarantia?.Any() == true) await context.GarantiaFotos.AddRangeAsync(fotoGarantia);
                if (fotoNegocio?.Any() == true) await context.FotoNegocios.AddRangeAsync(fotoNegocio);

                credito.IdPersona = persona.IdPersona;
                credito.IdConyuge = conyuge?.IdConyuge;
                credito.IdFiador = fiador?.IdFiador;
                credito.IdNegocio = negocio?.IdNegocio;
                credito.IdDocumentacion = documentacion.IdDocumentacion;
                credito.IdGarantia = garantia.IdGarantia;
                credito.IdCapacidadPago = capacidadPago?.IdCapacidadPago;

                var calendarioCond = new CredCalendCond
                {
                    NDiaFijo = 1,
                    NCuotas = credito.NNroCuotas,
                    NPlazo = credito.NPeriodo,
                    NNroCalen = 1,
                    BCobroSab = true,
                    BCobroDom = false,
                    BCobroFer = false,
                    BCuotaDoble = false,
                    IdCalenGasto = 1
                };

                await context.CredCalendConds.AddAsync(calendarioCond);
                await context.SaveChangesAsync();
                credito.IdCredCalendCond = calendarioCond.IdCredCalendCond;

                var correo = string.IsNullOrWhiteSpace(persona.CCorreo)
                    ? $"{persona.CDocumento}@crediavanza.com"
                    : persona.CCorreo;
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                var random = new Random();

                var passwordTemporal = new string(
                    Enumerable.Repeat(chars, 6)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray()
                );
                var token = Random.Shared.Next(0, 1_000_000).ToString("D6");
                var tokenInt = int.Parse(token);

                // Add UsuarioLogin for the persona
                var usuario = new UsuarioLogin
                {
                    CDocumento = persona.CDocumento,
                    IdPersona = persona.IdPersona,
                    CCorreo = correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(passwordTemporal),
                    Token = tokenInt,
                    TokenTime = DateTime.UtcNow,
                    Estado = 1,
                    IntentosFallidos = 0,
                    Bloqueado = 0,
                    TokenCheck = false,
                    BContrasenaTemporal = true,
                    DFechaContrasenaTemporal = DateTime.UtcNow
                };

                await context.UsuarioLogins.AddAsync(usuario);
                // Save now to get IdUsuario assigned so we can create UsuarioRole
                await context.SaveChangesAsync();

                // Ensure default role 'Usuario' exists and assign to the new user
                var defaultRoleName = "Usuario";
                var defaultRoleDesc = "Usuario estándar";

                var role = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == defaultRoleName);
                if (role == null)
                {
                    role = new Role
                    {
                        Nombre = defaultRoleName,
                        Descripcion = defaultRoleDesc,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    };
                    await context.Roles.AddAsync(role);
                    await context.SaveChangesAsync();
                }

                var usuarioRole = new UsuarioRole
                {
                    IdUsuario = usuario.IdUsuario,
                    IdRol = role.IdRol,
                    FechaAsignacion = DateTime.UtcNow
                };

                await context.UsuarioRoles.AddAsync(usuarioRole);

                await context.Creditos.AddAsync(credito);

                int result = await context.SaveChangesAsync();

                await calendarioService.GenerarCalendarioAsync(credito.NCodAge, credito.NCodCred);

                var subject = "Solicitud de crédito recibida";
                var nombre = string.IsNullOrWhiteSpace(persona.CNombres + ' ' + persona.CPrimerApellido)
                    ? persona.CDocumento
                    : persona.CNombres;
                var body = EmailTemplates.SolicitudCredito(nombre, tokenInt, passwordTemporal);

                await emailService.SendAsync(correo, subject, body);

                await tx.CommitAsync();

                return result;
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