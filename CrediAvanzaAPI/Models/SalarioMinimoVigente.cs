using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class SalarioMinimoVigente
{
    public int NCodSalMin { get; set; }

    public string CSector { get; set; } = null!;

    public decimal NMontoMensual { get; set; }

    public DateOnly DFecInicio { get; set; }

    public DateOnly? DFecFin { get; set; }

    public bool BEstado { get; set; }
}
