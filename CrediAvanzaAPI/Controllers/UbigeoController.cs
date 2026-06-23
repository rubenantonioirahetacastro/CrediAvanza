using System.Linq;
using CrediAvanzaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UbigeoController : ControllerBase
{
    private readonly DbNegocioContext _context;

    public UbigeoController(DbNegocioContext context)
    {
        _context = context;
    }

    // GET: api/ubigeo/departamentos
    [HttpGet("departamentos")]
    public async Task<IActionResult> GetDepartamentos()
    {
        var deps = await _context.Departamentos
            .AsNoTracking()
            .OrderBy(d => d.CNombre)
            .Select(d => new { d.IdDepartamento, d.CNombre })
            .ToListAsync();

        return Ok(deps);
    }

    // GET: api/ubigeo/departamentos/{id}
    [HttpGet("departamentos/{id}")]
    public async Task<IActionResult> GetDepartamento(int id)
    {
        var dep = await _context.Departamentos
            .AsNoTracking()
            .Where(d => d.IdDepartamento == id)
            .Select(d => new { d.IdDepartamento, d.CNombre })
            .FirstOrDefaultAsync();

        if (dep == null) return NotFound();
        return Ok(dep);
    }

    // GET: api/ubigeo/municipios/{idDepartamento}
    [HttpGet("municipios/{idDepartamento}")]
    public async Task<IActionResult> GetMunicipios(int idDepartamento)
    {
        var mun = await _context.Municipios
            .AsNoTracking()
            .Where(m => m.IdDepartamento == idDepartamento)
            .OrderBy(m => m.CNombre)
            .Select(m => new { m.IdMunicipio, m.CNombre, m.IdDepartamento })
            .ToListAsync();

        return Ok(mun);
    }

    // GET: api/ubigeo/municipios/id/{id}
    [HttpGet("municipios/id/{id}")]
    public async Task<IActionResult> GetMunicipio(int id)
    {
        var mun = await _context.Municipios
            .AsNoTracking()
            .Where(m => m.IdMunicipio == id)
            .Select(m => new { m.IdMunicipio, m.CNombre, m.IdDepartamento })
            .FirstOrDefaultAsync();

        if (mun == null) return NotFound();
        return Ok(mun);
    }

    // POST: api/ubigeo/departamentos
    [HttpPost("departamentos")]
    public async Task<IActionResult> CreateDepartamento([FromBody] Departamento departamento)
    {
        if (departamento == null || string.IsNullOrWhiteSpace(departamento.CNombre))
            return BadRequest("Departamento inválido");

        _context.Departamentos.Add(departamento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDepartamento), new { id = departamento.IdDepartamento }, departamento);
    }

    // PUT: api/ubigeo/departamentos/{id}
    [HttpPut("departamentos/{id}")]
    public async Task<IActionResult> UpdateDepartamento(int id, [FromBody] Departamento departamento)
    {
        if (departamento == null || id != departamento.IdDepartamento)
            return BadRequest();

        var existing = await _context.Departamentos.FindAsync(id);
        if (existing == null) return NotFound();

        existing.CNombre = departamento.CNombre;

        _context.Departamentos.Update(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/ubigeo/departamentos/{id}
    [HttpDelete("departamentos/{id}")]
    public async Task<IActionResult> DeleteDepartamento(int id)
    {
        var existing = await _context.Departamentos.Include(d => d.Municipios).FirstOrDefaultAsync(d => d.IdDepartamento == id);
        if (existing == null) return NotFound();

        // cascade delete configured in model; ensure related municipios removed
        _context.Departamentos.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/ubigeo/municipios
    [HttpPost("municipios")]
    public async Task<IActionResult> CreateMunicipio([FromBody] Municipio municipio)
    {
        if (municipio == null || string.IsNullOrWhiteSpace(municipio.CNombre))
            return BadRequest("Municipio inválido");

        // verify departamento exists
        var dep = await _context.Departamentos.FindAsync(municipio.IdDepartamento);
        if (dep == null) return BadRequest("Departamento no existe");

        _context.Municipios.Add(municipio);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMunicipio), new { id = municipio.IdMunicipio }, municipio);
    }

    // PUT: api/ubigeo/municipios/{id}
    [HttpPut("municipios/{id}")]
    public async Task<IActionResult> UpdateMunicipio(int id, [FromBody] Municipio municipio)
    {
        if (municipio == null || id != municipio.IdMunicipio) return BadRequest();

        var existing = await _context.Municipios.FindAsync(id);
        if (existing == null) return NotFound();

        // verify departamento exists
        var dep = await _context.Departamentos.FindAsync(municipio.IdDepartamento);
        if (dep == null) return BadRequest("Departamento no existe");

        existing.CNombre = municipio.CNombre;
        existing.IdDepartamento = municipio.IdDepartamento;

        _context.Municipios.Update(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/ubigeo/municipios/{id}
    [HttpDelete("municipios/{id}")]
    public async Task<IActionResult> DeleteMunicipio(int id)
    {
        var existing = await _context.Municipios.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Municipios.Remove(existing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
