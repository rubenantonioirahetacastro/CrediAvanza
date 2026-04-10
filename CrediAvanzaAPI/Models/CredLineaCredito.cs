using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredLineaCredito
{
    public int NCodLinea { get; set; }

    public string CDescLinea { get; set; } = null!;

    public decimal NTasaCom { get; set; }

    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public int NPlazoMin { get; set; }

    public int NPlazoMax { get; set; }

    public decimal NMontoMin { get; set; }

    public decimal NMontoMax { get; set; }

    public bool BEstado { get; set; }
}
