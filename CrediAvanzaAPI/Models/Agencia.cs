using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Agencia
{
    public int NCodAge { get; set; }

    public string CNomAge { get; set; } = null!;

    public string CDirecAge { get; set; } = null!;

    public string CTelefAge { get; set; } = null!;

    public string CCorreoElectronico { get; set; } = null!;
}
