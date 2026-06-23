using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonaController : ControllerBase
{
    private readonly DbNegocioContext _context;

    public PersonaController(DbNegocioContext context)
    {
        _context = context;
    }

    // PUT: api/persona/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePersona(int id, [FromBody] Persona persona)
    {
        if (persona == null) return BadRequest("Persona inválida");

        if (persona.IdPersona != 0 && persona.IdPersona != id)
            return BadRequest("El IdPersona del cuerpo no coincide con el id de la ruta");

        var existing = await _context.Personas.FindAsync(id);
        if (existing == null) return NotFound($"Persona {persona.CNombres} no existe en la base de datos ");

        // Actualizar campos permitidos
        existing.NTipoDocumento = persona.NTipoDocumento;
        existing.CDocumento = persona.CDocumento;
        existing.DFechaExpedicion = persona.DFechaExpedicion;
        existing.DFechaVencimiento = persona.DFechaVencimiento;
        existing.NDepartamentoDoc = persona.NDepartamentoDoc;
        existing.NMunicipioDoc = persona.NMunicipioDoc;
        existing.CNombres = persona.CNombres;
        existing.CPrimerApellido = persona.CPrimerApellido;
        existing.CSegundoApellido = persona.CSegundoApellido;
        existing.NSexo = persona.NSexo;
        existing.NNacionalidad = persona.NNacionalidad;
        existing.DFechaNacimiento = persona.DFechaNacimiento;
        existing.NDepartamentoNacimiento = persona.NDepartamentoNacimiento;
        existing.NMunicipioNacimiento = persona.NMunicipioNacimiento;
        existing.NEstadoCivil = persona.NEstadoCivil;
        existing.NProfesion = persona.NProfesion;
        existing.NEscolaridad = persona.NEscolaridad;
        existing.CCorreo = persona.CCorreo;
        existing.NTelefono = persona.NTelefono;
        existing.NCelular = persona.NCelular;

        _context.Personas.Update(existing);
        await _context.SaveChangesAsync();

        return Ok("Datos actualizados correctamente");
    }
}
