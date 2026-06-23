using System;
using System.Collections.Generic;

namespace CrediAvanzaAPI.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string CNombre { get; set; } = null!;

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}
