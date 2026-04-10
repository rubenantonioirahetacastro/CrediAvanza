using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredCalendario
{
    public int IdCalendario { get; set; }

    public int NNroCuota { get; set; }

    public DateTime DFecVenc { get; set; }

    public DateTime? DFecPago { get; set; }

    public decimal NCapital { get; set; }

    public decimal NIntComp { get; set; }

    public decimal NIntMor { get; set; }

    public decimal NIgv { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal nTotalCuota { get; set; }

    public decimal NCapPag { get; set; }

    public decimal NIntPag { get; set; }

    public decimal NIntMorPag { get; set; }

    public decimal NIgvPag { get; set; }

    public int NEstado { get; set; }

    public int NNroCalen { get; set; }

    public DateTime? DFecpro { get; set; }

    public int? NTipoCargo { get; set; }
}
