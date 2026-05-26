using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class FotoGarantium
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public int IdTipoGarantia { get; set; }

    public int IdGarantia { get; set; }
}
