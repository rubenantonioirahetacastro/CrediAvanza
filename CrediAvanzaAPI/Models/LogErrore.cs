using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class LogErrore
{
    public int IdLogError { get; set; }

    public string Origen { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string? StackTrace { get; set; }

    public string? TipoExcepcion { get; set; }

    public string? DatosAdicionales { get; set; }

    public string? Usuario { get; set; }

    public string? Ip { get; set; }

    public DateTime FechaError { get; set; }
}
