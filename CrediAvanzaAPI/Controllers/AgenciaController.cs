using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciaController : ControllerBase
    {
        private readonly IAgenciaService _agenciaService;

        public AgenciaController(IAgenciaService agenciaService)
        {
            _agenciaService = agenciaService;
        }

        [HttpGet]
        public async Task<IActionResult> AllAgencias()
        {
            var agencias = await _agenciaService.AllAgencias();
            return Ok(agencias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgenciaById(int id)
        {
            var agencia = await _agenciaService.GetAgenciaById(id);
            if (agencia == null)
                return NotFound("Agencia no encontrada");

            return Ok(agencia);
        }

        [HttpPost]
        public async Task<IActionResult> AddAgencia([FromBody] Agencia agencia)
        {
            try
            {
                await _agenciaService.AddAgencia(agencia);
                return CreatedAtAction(nameof(GetAgenciaById), new { id = agencia.NCodAge }, agencia);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAgencia([FromBody] Agencia agencia)
        {
            try
            {
                var updated = await _agenciaService.UpdateAgencia(agencia);
                if (!updated)
                    return NotFound("Agencia no encontrada para actualizar");

                return Ok("Actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgencia(int id)
        {
            var deleted = await _agenciaService.DeleteAgencia(id);
            if (!deleted)
                return NotFound("Agencia no encontrada para eliminar");

            return Ok("Eliminado correctamente.");
        }
    }
}