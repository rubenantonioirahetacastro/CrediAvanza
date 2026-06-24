using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class PasswordChangeAudit
{
    public int IdAudit { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdPersona { get; set; }

    public string Usuario { get; set; } = null!;

    public bool Exito { get; set; }

    public DateTime FechaAttempt { get; set; }

    public string? Ip { get; set; }

    public string? UserAgent { get; set; }

    public int? IntentosFallidos { get; set; }

    public bool? Bloqueado { get; set; }

    public DateTime? FechaBloqueo { get; set; }

    public string? MotivoBloqueo { get; set; }

    public string? Observacion { get; set; }
}
