using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public string CProducto { get; set; } = null!;

    public int NCantidadCompra { get; set; }

    public int NUnidadMedida { get; set; }

    public decimal NPrecioXunidad { get; set; }

    public decimal NPrecioTotal { get; set; }

    public int IdNegocio { get; set; }
}
