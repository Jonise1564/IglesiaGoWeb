using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Data;
using IglesiaGo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IglesiaGo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere Token JWT para todas las operaciones
    public class ConsultasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsultasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ConsultasApi
        // Retorna el listado de consultas (cabeceras)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultas()
        {
            try
            {
                var consultas = await _context.Consultas
                    .OrderByDescending(c => c.FechaCreacion)
                    .ToListAsync();

                return Ok(consultas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al recuperar consultas", error = ex.Message });
            }
        }

        // GET: api/ConsultasApi/5
        // Retorna una consulta con todo su hilo de mensajes (mensajes_consulta)
        [HttpGet("{id}")]
        public async Task<ActionResult<Consulta>> GetConsulta(int id)
        {
            // Usamos .Include para traer los mensajes relacionados
            var consulta = await _context.Consultas
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null) return NotFound();

            return Ok(consulta);
        }

        // POST: api/ConsultasApi
        // Para que la App envíe una consulta nueva
       [AllowAnonymous] // Permite que esta acción sea accesible sin autenticación
        [HttpPost]
        public async Task<ActionResult<Consulta>> PostConsulta(Consulta consulta)
        {
            try
            {
                consulta.FechaCreacion = DateTime.Now;
                consulta.Atendido = false;

                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetConsulta), new { id = consulta.Id }, consulta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al enviar la consulta", error = ex.Message });
            }
        }

        // POST: api/ConsultasApi/Responder
        
        [HttpPost("Responder")]
        public async Task<IActionResult> PostMensajeHilo([FromBody] MensajeConsulta nuevoMensaje)
        {
            // 1. Forzamos la limpieza de validaciones automáticas
            ModelState.Clear();

            // 2. Verificación de seguridad
            if (nuevoMensaje == null)
            {
                return BadRequest(new { mensaje = "El servidor recibió un objeto nulo." });
            }

            try
            {
                // 3. Aseguramos datos mínimos
                nuevoMensaje.FechaEnvio = DateTime.Now;
                nuevoMensaje.Consulta = null; // Evita que intente validar la relación

                _context.MensajesConsulta.Add(nuevoMensaje);

                // 4. Actualizar estado de la consulta padre
                var consultaOriginal = await _context.Consultas.FindAsync(nuevoMensaje.ConsultaId);
                if (consultaOriginal != null)
                {
                    consultaOriginal.Atendido = true;
                }

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Respuesta guardada con éxito" });
            }
            catch (Exception ex)
            {
                // Esto te dirá si el problema es MySQL (ej: falta una columna)
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        // DELETE: api/ConsultasApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsulta(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null) return NotFound();

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Consulta eliminada correctamente" });
        }
    }
}