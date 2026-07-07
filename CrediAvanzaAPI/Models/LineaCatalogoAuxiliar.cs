using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class LineaCatalogoAuxiliar
{
    public int Id { get; set; }

    public string? CDescripcion { get; set; }

    public int? NCatalogoCodigo { get; set; }

    public int? NProd { get; set; }

    public int? NSubProd { get; set; }
}
