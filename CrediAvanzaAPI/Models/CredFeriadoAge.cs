using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredFeriadoAge
{
    public int IdCredFeriadoAge { get; set; }

    public int NIdFeriado { get; set; }

    public int NCodAge { get; set; }

    public DateTime DFecha { get; set; }
}
