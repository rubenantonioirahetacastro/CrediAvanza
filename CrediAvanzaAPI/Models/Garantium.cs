using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Garantium
{
    public int IdGarantia { get; set; }

    public int NIdArticuloGarantia { get; set; }

    public decimal NValor { get; set; }

    public int NIdFotoGarantia { get; set; }
}
