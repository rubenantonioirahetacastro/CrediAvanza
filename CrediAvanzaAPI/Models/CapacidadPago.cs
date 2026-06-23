using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class CapacidadPago
{
    public int IdCapacidadPago { get; set; }

    public decimal DGastosEducacion { get; set; }

    public decimal DGastosAlimentacion { get; set; }

    public decimal DGastosSalud { get; set; }

    public decimal DOtrosGastos { get; set; }

    public decimal DOtrosIngresos { get; set; }
}
