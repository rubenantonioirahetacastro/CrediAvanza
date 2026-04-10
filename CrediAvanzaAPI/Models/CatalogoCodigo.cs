using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CatalogoCodigo
{
    public int NCodigo { get; set; }

    public int NValor { get; set; }

    public string CNomCod { get; set; } = null!;

    public int? NEstados { get; set; }

    public int? NTipoCodigo { get; set; }
}
