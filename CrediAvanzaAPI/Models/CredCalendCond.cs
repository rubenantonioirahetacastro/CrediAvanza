using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredCalendCond
{
    public int IdCredCalendCond { get; set; }

    public int? NDiaFijo { get; set; }

    public int NCuotas { get; set; }

    public int NPlazo { get; set; }

    public int NNroCalen { get; set; }

    public bool? BCobroSab { get; set; }

    public bool? BCobroDom { get; set; }

    public bool? BCobroFer { get; set; }

    public bool? BCuotaDoble { get; set; }

    public int? IdCalenGasto { get; set; }
}
