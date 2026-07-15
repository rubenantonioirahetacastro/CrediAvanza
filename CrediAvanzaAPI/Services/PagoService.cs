using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Services
{
    public class PagoService : IPagoService
    {
        private readonly DbNegocioContext _context;

        public PagoService(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task<List<CredCalendario>> RegistrarPagoAsync(PagoRequest request)
        {
            if (request.MontoAbonado <= 0)
                throw new ArgumentException("El monto a abonar debe ser mayor a cero.");

            var credito = await _context.Creditos
                .FirstOrDefaultAsync(c => c.NCodAge == request.NCodAge && c.NCodCred == request.NCodCred)
                ?? throw new Exception("Crédito no encontrado");

            // Obtener el calendario cond (condición de calendario actual)
            if (credito.IdCredCalendCond == null)
                throw new Exception("El crédito no tiene una condición de calendario asociada.");

            var calendCond = await _context.CredCalendConds
                .FirstOrDefaultAsync(cc => cc.IdCredCalendCond == credito.IdCredCalendCond.Value)
                ?? throw new Exception("Condición de calendario no encontrada.");

            int nroCalenActual = calendCond.NNroCalen;

            var cuotasPendientes = await _context.CredCalendarios
                .Where(c => c.NCodAge == request.NCodAge && 
                            c.NCodCred == request.NCodCred && 
                            c.NNroCalen == nroCalenActual && 
                            c.NEstado != 1)
                .OrderBy(c => c.NNroCuota)
                .ToListAsync();

            if (!cuotasPendientes.Any())
                throw new Exception("El crédito no tiene cuotas pendientes en el calendario actual.");

            var lineaCredito = await _context.CredLineaCreditos
                .FirstOrDefaultAsync(l => l.NCodLinea == credito.NCodLinea);
            decimal tasaInteresMensual = lineaCredito != null ? lineaCredito.NTasaCom : 0m;

            decimal montoRestante = request.MontoAbonado;
            DateTime fechaPago = DateTime.Now;
            var cuotasPagadas = new List<CredCalendario>();

            // 1. Determinar la cuota actual (la vencida o la más inmediata)
            var cuotasActualesYVencidas = cuotasPendientes
                .Where(c => c.DFecVenc.Date <= fechaPago.Date)
                .ToList();

            if (!cuotasActualesYVencidas.Any())
                cuotasActualesYVencidas.Add(cuotasPendientes.First());

            // Procesar las cuotas pendientes / actuales primero
            foreach (var cuota in cuotasActualesYVencidas)
            {
                if (montoRestante <= 0) break;

                decimal difMor = Math.Max(0, cuota.NIntMor - cuota.NIntMorPag);
                decimal difIgv = Math.Max(0, cuota.NIgv - cuota.NIgvPag);
                decimal difInt = Math.Max(0, cuota.NIntComp - cuota.NIntPag);
                decimal difCap = Math.Max(0, cuota.NCapital - cuota.NCapPag);

                decimal pagadoMor = Math.Min(montoRestante, difMor);
                cuota.NIntMorPag += pagadoMor; montoRestante -= pagadoMor;

                decimal pagadoIgv = Math.Min(montoRestante, difIgv);
                cuota.NIgvPag += pagadoIgv; montoRestante -= pagadoIgv;

                decimal pagadoInt = Math.Min(montoRestante, difInt);
                cuota.NIntPag += pagadoInt; montoRestante -= pagadoInt;

                decimal pagadoCap = Math.Min(montoRestante, difCap);
                cuota.NCapPag += pagadoCap; montoRestante -= pagadoCap;
                credito.NSaldoK -= pagadoCap;

                cuota.DFecPago = fechaPago;
                if (cuota.NCapital <= cuota.NCapPag && cuota.NIntComp <= cuota.NIntPag &&
                    cuota.NIgv <= cuota.NIgvPag && cuota.NIntMor <= cuota.NIntMorPag)
                {
                    cuota.NEstado = 1; // Pagado Totalmente
                }
                else
                {
                    cuota.NEstado = 2; // Pago Parcial
                }

                cuotasPagadas.Add(cuota);
            }

            // 2. Si sobra montoRestante, agregarlo a las siguientes cuotas (solo capital e IGV) hasta donde alcance
            var cuotasAnticipadas = cuotasPendientes.Except(cuotasActualesYVencidas).OrderBy(c => c.NNroCuota).ToList();

            if (montoRestante > 0 && cuotasAnticipadas.Any())
            {
                foreach (var c in cuotasAnticipadas)
                {
                    if (montoRestante <= 0) break;

                    decimal difIgv = Math.Max(0, c.NIgv - c.NIgvPag);
                    decimal difCap = Math.Max(0, c.NCapital - c.NCapPag);

                    decimal abonoIgv = Math.Min(montoRestante, difIgv);
                    c.NIgvPag += abonoIgv;
                    montoRestante -= abonoIgv;

                    decimal abonoCap = Math.Min(montoRestante, difCap);
                    c.NCapPag += abonoCap;
                    montoRestante -= abonoCap;

                    credito.NSaldoK -= abonoCap;
                    if (credito.NSaldoK < 0) credito.NSaldoK = 0;

                    c.DFecPago = fechaPago;

                    // Cuando se completa una cuota sería estado 1 de lo contrario 0
                    if (c.NCapital <= c.NCapPag && c.NIgv <= c.NIgvPag)
                    {
                        c.NEstado = 1; // Pagado Totalmente
                        c.NIntComp = c.NIntPag; 
                        c.NIntMor = c.NIntMorPag;
                    }
                    else
                    {
                        c.NEstado = 0; // Parcial o pendiente
                    }

                    if (!cuotasPagadas.Contains(c))
                        cuotasPagadas.Add(c);
                }

                if (montoRestante > 0)
                {
                    credito.NSaldoK -= montoRestante;
                    if (credito.NSaldoK < 0) credito.NSaldoK = 0;
                    montoRestante = 0;
                }
            }
            else if (montoRestante > 0 && credito.NSaldoK > 0)
            {
                // Si no hay cuotas anticipadas pero hay sobrante, abonarlo al saldo general
                credito.NSaldoK -= montoRestante;
                montoRestante = 0;
                if (credito.NSaldoK < 0) credito.NSaldoK = 0;
            }

            if (credito.NSaldoK <= 0)
            {
                credito.NEstado = 1; // Pagado Total/Cancelado
                // Si quedaron cuotas sin pagar, ponerlas en cero
                foreach(var future in cuotasPendientes.Where(c => c.NEstado != 1))
                {
                    if (!cuotasPagadas.Contains(future))
                    {
                        future.NCapital = 0;
                        future.NIntComp = 0;
                        future.NIgv = 0;
                        future.NTotalCuota = 0;
                        future.NEstado = 1; 
                    }
                }
            }

            await _context.SaveChangesAsync();
            return cuotasPagadas;
        }
    }
}