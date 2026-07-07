using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CatSegmentoUsura
{
    public int NCodSegmento { get; set; }

    public string CDescripcion { get; set; } = null!;

    public decimal NMultSmmin { get; set; }

    public decimal? NMultSmmax { get; set; }

    public bool BEstado { get; set; }

    public virtual ICollection<TasaMaximaBcr> TasaMaximaBcrs { get; set; } = new List<TasaMaximaBcr>();
}
