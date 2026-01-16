using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class VerNegocio
{
    public int NCodVar { get; set; }

    public string CNomVar { get; set; } = null!;

    public string CValorVar { get; set; } = null!;

    public int NTipoVar { get; set; }
}
