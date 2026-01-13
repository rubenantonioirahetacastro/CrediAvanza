using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredCalenGasto
{
    public int IdCalenGasto { get; set; }

    public int NNroCuota { get; set; }

    public int NCodGasto { get; set; }

    public decimal NMonto { get; set; }

    public decimal NMontoPag { get; set; }

    public DateTime DFecAsig { get; set; }

    public int? NCodAgePersAsig { get; set; }

    public int NNroCalen { get; set; }

    public decimal? NMontoIgv { get; set; }

    public decimal? NMontoIgvpag { get; set; }

    public decimal? NMontoSinIgv { get; set; }

    public decimal? NMontoSinIgvpag { get; set; }
}
