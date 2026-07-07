using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class TasaMaximaBcr
{
    public int NCodTasaMax { get; set; }

    public int NCodSegmento { get; set; }

    public decimal NTasaMaxima { get; set; }

    public DateOnly DFecInicio { get; set; }

    public DateOnly DFecFin { get; set; }

    public DateOnly DFecPublicacion { get; set; }

    public bool BEstado { get; set; }

    public virtual CatSegmentoUsura NCodSegmentoNavigation { get; set; } = null!;
}
