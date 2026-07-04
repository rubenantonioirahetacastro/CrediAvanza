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
        existing.CTelefono = persona.CTelefono;
        existing.CCelular = persona.CCelular;

        _context.Personas.Update(existing);
        await _context.SaveChangesAsync();

        return Ok("Datos actualizados correctamente");
    }

    // PUT: api/persona/{id}/contacto
    [HttpPut("{id}/contacto")]
    public async Task<IActionResult> UpdatePersonaContacto(int id, [FromBody] UpdatePersonaContactoRequest contacto)
    {
        if (contacto == null) return BadRequest("No se pudo completar la actualización porque el cuerpo de la solicitud es nulo");

        var existing = await _context.Personas.FindAsync(id);
        if (existing == null) return NotFound($"Persona {id} no existe en la base de datos");

        existing.CCorreo = contacto.CCorreo;
        existing.CTelefono = contacto.CTelefono;
        existing.CCelular = contacto.CCelular;

        _context.Personas.Update(existing);
        await _context.SaveChangesAsync();

        return Ok("Datos de contacto actualizados correctamente");
    }
}

public sealed record UpdatePersonaContactoRequest(string? CCorreo, string CTelefono, string CCelular);
