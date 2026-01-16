using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredCambioGasto
{
    public int NIdCambio { get; set; }

    public int NCodCred { get; set; }

    public DateOnly DFechaCambio { get; set; }

    public int IdUsuario { get; set; }

    public decimal NMontoOriginal { get; set; }

    public decimal NMontoNuevo { get; set; }
}
