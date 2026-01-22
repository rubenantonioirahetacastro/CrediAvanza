using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public int NTipoDocumento { get; set; }

    public string CDocumento { get; set; } = null!;

    public DateOnly DFechaExpedicion { get; set; }

    public DateOnly DFechaVencimiento { get; set; }

    public int NDepartamentoDoc { get; set; }

    public int NMunicipioDoc { get; set; }

    public string CNombres { get; set; } = null!;

    public string CPrimerApellido { get; set; } = null!;

    public string? CSegundoApellido { get; set; }

    public int NSexo { get; set; }

    public int NNacionalidad { get; set; }

    public DateOnly DFechaNacimiento { get; set; }

    public int NDepartamentoNacimiento { get; set; }

    public int NMunicipioNacimiento { get; set; }

    public int NEstadoCivil { get; set; }

    public int NProfesion { get; set; }

    public int NEscolaridad { get; set; }

    public string? CCorreo { get; set; }
}
