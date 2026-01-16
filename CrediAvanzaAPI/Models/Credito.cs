using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Credito
{
    public int NCodCred { get; set; }

    public int NCodAge { get; set; }

    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public decimal NSaldoK { get; set; }

    public decimal NPrestamo { get; set; }

    public DateTime DFecVig { get; set; }

    public int? NCodLinea { get; set; }

    public int NEstado { get; set; }

    public int NPeriodo { get; set; }

    public decimal? NMontoCuota { get; set; }

    public decimal? NTasaComp { get; set; }

    public decimal? NTasaMor { get; set; }

    public int NNroCuotas { get; set; }

    public int NNroProxCuota { get; set; }

    public decimal? NMora { get; set; }

    public int? NDiasAtraso { get; set; }

    public int? NCiclo { get; set; }

    public decimal? NTasaComision { get; set; }

    public int? NCobroEnAgencia { get; set; }

    public int? NAceptaTerminos { get; set; }

    public int? IdPersona { get; set; }

    public int? IdCompra { get; set; }

    public int? IdConyuge { get; set; }

    public int? IdVenta { get; set; }

    public int? IdDocumentacion { get; set; }

    public int? IdGarantia { get; set; }

    public int? IdFiador { get; set; }

    public int? IdCalendario { get; set; }

    public int? IdCredCalendCond { get; set; }

    public int? IdNegocio { get; set; }
}
