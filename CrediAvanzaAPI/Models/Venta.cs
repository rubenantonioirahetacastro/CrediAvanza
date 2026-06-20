using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public string CProducto { get; set; } = null!;

    public decimal NCantidadVenta { get; set; }

    public int NUnidadMedida { get; set; }

    public decimal NPrecioXunidad { get; set; }

    public decimal NPrecioTotal { get; set; }

    public int IdNegocio { get; set; }
}
