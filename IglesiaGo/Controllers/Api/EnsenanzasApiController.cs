using IglesiaGo.Data;
using IglesiaGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IglesiaGo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnsenanzasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnsenanzasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: api/EnsenanzasApi
        // Listar todas las enseñanzas
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enseñanza>>> GetEnsenanzas()
        {
            var ensenanzas = await _context.Enseñanzas
                .AsNoTracking()
                .OrderByDescending(e => e.FechaPublicacion)
                .ToListAsync();

            return Ok(ensenanzas);
        }

        // ============================================================
        // GET: api/EnsenanzasApi/5
        // Obtener una enseñanza por Id
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Enseñanza>> GetEnsenanza(int id)
        {
            var ensenanza = await _context.Enseñanzas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ensenanza == null)
                return NotFound(new
                {
                    mensaje = "La enseñanza no existe."
                });

            return Ok(ensenanza);
        }

        // ============================================================
        // POST: api/EnsenanzasApi
        // Crear una enseñanza
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<Enseñanza>> PostEnsenanza([FromBody] Enseñanza ensenanza)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (ensenanza.FechaPublicacion == DateTime.MinValue)
                ensenanza.FechaPublicacion = DateTime.Now;

            _context.Enseñanzas.Add(ensenanza);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnsenanza),
                new { id = ensenanza.Id },
                ensenanza);
        }

        // ============================================================
        // PUT: api/EnsenanzasApi/5
        // Editar una enseñanza
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnsenanza(int id, [FromBody] Enseñanza model)
        {
            if (id != model.Id)
                return BadRequest(new
                {
                    mensaje = "El Id no coincide."
                });

            var ensenanza = await _context.Enseñanzas.FindAsync(id);

            if (ensenanza == null)
                return NotFound(new
                {
                    mensaje = "La enseñanza no existe."
                });

            ensenanza.Titulo = model.Titulo;
            ensenanza.Contenido = model.Contenido;
            ensenanza.VideoUrl = model.VideoUrl;
            ensenanza.FechaPublicacion = model.FechaPublicacion;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Enseñanza actualizada correctamente.",
                data = ensenanza
            });
        }

        // ============================================================
        // DELETE: api/EnsenanzasApi/5
        // Eliminar una enseñanza
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnsenanza(int id)
        {
            var ensenanza = await _context.Enseñanzas.FindAsync(id);

            if (ensenanza == null)
                return NotFound(new
                {
                    mensaje = "La enseñanza no existe."
                });

            _context.Enseñanzas.Remove(ensenanza);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Enseñanza eliminada correctamente."
            });
        }
    }
}