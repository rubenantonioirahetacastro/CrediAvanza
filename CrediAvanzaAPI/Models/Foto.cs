using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Foto
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public int NTipoFoto { get; set; }
}
