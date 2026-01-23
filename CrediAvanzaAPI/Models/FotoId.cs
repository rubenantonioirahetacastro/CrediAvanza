using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class FotoId
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public int NTipoFoto { get; set; }

    public int IdPersona { get; set; }
}
