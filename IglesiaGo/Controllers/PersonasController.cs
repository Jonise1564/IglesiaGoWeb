using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IglesiaGo.Data;
using IglesiaGo.Models;

namespace IglesiaGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/personas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas([FromQuery] string? tipo)
        {
            var query = _context.Personas.AsQueryable();

            // Filtro dinámico por TipoPersona (ej: api/personas?tipo=Músico)
            if (!string.IsNullOrEmpty(tipo))
            {
                query = query.Where(p => p.TipoPersona == tipo);
            }

            return await query.ToListAsync();
        }

        // GET: api/personas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Persona>> GetPersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);

            if (persona == null)
            {
                return NotFound(new { mensaje = "Persona no encontrada" });
            }

            return persona;
        }

        // POST: api/personas
        [HttpPost]
        public async Task<ActionResult<Persona>> PostPersona(PersonaUpsertDto dto)
        {
            // Validar si el DocumentoIdentidad ya existe para evitar duplicados en la DB
            if (await _context.Personas.AnyAsync(p => p.DocumentoIdentidad == dto.DocumentoIdentidad))
            {
                return BadRequest(new { mensaje = "El Documento de Identidad ya se encuentra registrado" });
            }

            var nuevaPersona = new Persona
            {
                DocumentoIdentidad = dto.DocumentoIdentidad,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                Telefono = dto.Telefono,
                TelefonoAlternativo = dto.TelefonoAlternativo,
                FechaNacimiento = dto.FechaNacimiento,
                Genero = dto.Genero,
                EstadoCivil = dto.EstadoCivil,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                EstadoProvincia = dto.EstadoProvincia,
                CodigoPostal = dto.CodigoPostal,
                Pais = dto.Pais,
                TipoPersona = dto.TipoPersona,
                UsuarioId = dto.UsuarioId,
                Activo = 1
            };

            _context.Personas.Add(nuevaPersona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPersona), new { id = nuevaPersona.Id }, nuevaPersona);
        }

        // PUT: api/personas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(int id, PersonaUpsertDto dto)
        {
            var personaExistente = await _context.Personas.FindAsync(id);

            if (personaExistente == null)
            {
                return NotFound(new { mensaje = "Persona no encontrada" });
            }

            // Actualizamos todos los campos con las propiedades correctas
            personaExistente.DocumentoIdentidad = dto.DocumentoIdentidad;
            personaExistente.Nombres = dto.Nombres;
            personaExistente.Apellidos = dto.Apellidos;
            personaExistente.Email = dto.Email;
            personaExistente.Telefono = dto.Telefono;
            personaExistente.TelefonoAlternativo = dto.TelefonoAlternativo;
            personaExistente.FechaNacimiento = dto.FechaNacimiento;
            personaExistente.Genero = dto.Genero;
            personaExistente.EstadoCivil = dto.EstadoCivil;
            personaExistente.Direccion = dto.Direccion;
            personaExistente.Ciudad = dto.Ciudad;
            personaExistente.EstadoProvincia = dto.EstadoProvincia;
            personaExistente.CodigoPostal = dto.CodigoPostal;
            personaExistente.Pais = dto.Pais;
            personaExistente.TipoPersona = dto.TipoPersona;
            personaExistente.UsuarioId = dto.UsuarioId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/personas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound(new { mensaje = "Persona no encontrada" });
            }

            // Podés elegir removerla por completo u optar por borrado lógico usando: persona.Activo = 0;
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Persona eliminada correctamente" });
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.Id == id);
        }
    }
}