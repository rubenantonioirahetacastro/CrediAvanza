using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CredGasto
{
    public int IdGasto { get; set; }

    public int NProd { get; set; }

    public int NSubProd { get; set; }

    public string CDescripcion { get; set; } = null!;

    public int NTipoGasto { get; set; }

    public decimal NValor { get; set; }

    public decimal NRangoInicial { get; set; }

    public decimal NRangoFinal { get; set; }

    public int NPeriodo { get; set; }

    public int NTipoCargo { get; set; }
}
