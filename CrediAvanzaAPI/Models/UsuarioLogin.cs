using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class UsuarioLogin
{
    public int IdUsuario { get; set; }

    public string CDocumento { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string CCorreo { get; set; } = null!;

    public int? Token { get; set; }

    public DateTime? TokenTime { get; set; }

    public bool? TokenCheck { get; set; }

    public int? Estado { get; set; }

    public int? IntentosFallidos { get; set; }

    public int? Bloqueado { get; set; }

    public int? FechaBloqueo { get; set; }

    public DateTime? UltimoLogin { get; set; }
}
