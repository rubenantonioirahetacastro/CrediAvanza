using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class FotoDocumentacion
{
    public int IdFoto { get; set; }

    public string VFoto { get; set; } = null!;

    public int IdTipoDocumentacion { get; set; }

    public int IdDocumentacion { get; set; }
}
