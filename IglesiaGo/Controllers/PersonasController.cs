// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using IglesiaGo.Data;
// using IglesiaGo.Models;

// namespace IglesiaGo.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class PersonasController : ControllerBase
//     {
//         private readonly ApplicationDbContext _context;

//         public PersonasController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         // GET: api/personas
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas([FromQuery] string? tipo)
//         {
//             var query = _context.Personas.AsQueryable();

//             if (!string.IsNullOrEmpty(tipo))
//             {
//                 query = query.Where(p => p.TipoPersona == tipo);
//             }

//             return await query.ToListAsync();
//         }

//         // GET: api/personas/5
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Persona>> GetPersona(int id)
//         {
//             var persona = await _context.Personas.FindAsync(id);

//             if (persona == null)
//             {
//                 return NotFound(new { mensaje = "Persona no encontrada" });
//             }

//             return persona;
//         }

//         // POST: api/personas
//         [HttpPost]
//         public async Task<ActionResult<Persona>> PostPersona(PersonaUpsertDto dto)
//         {
//             if (await _context.Personas.AnyAsync(p => p.DocumentoIdentidad == dto.DocumentoIdentidad))
//             {
//                 return BadRequest(new { mensaje = "El Documento de Identidad ya se encuentra registrado" });
//             }

//             var nuevaPersona = new Persona
//             {
//                 DocumentoIdentidad = dto.DocumentoIdentidad,
//                 Nombres = dto.Nombres,
//                 Apellidos = dto.Apellidos,
//                 Email = dto.Email,
//                 Telefono = dto.Telefono,
//                 TelefonoAlternativo = dto.TelefonoAlternativo,
//                 FechaNacimiento = dto.FechaNacimiento,
//                 Genero = dto.Genero,
//                 EstadoCivil = dto.EstadoCivil,
//                 Direccion = dto.Direccion,
//                 Ciudad = dto.Ciudad,
//                 EstadoProvincia = dto.EstadoProvincia,
//                 CodigoPostal = dto.CodigoPostal,
//                 Pais = dto.Pais,
//                 TipoPersona = dto.TipoPersona,
//                 UsuarioId = dto.UsuarioId,
//                 Activo = 1 
//             };

//             _context.Personas.Add(nuevaPersona);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction(nameof(GetPersona), new { id = nuevaPersona.Id }, nuevaPersona);
//         }

//         // PUT: api/personas/5
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutPersona(int id, PersonaUpsertDto dto)
//         {
//             var personaExistente = await _context.Personas.FindAsync(id);

//             if (personaExistente == null)
//             {
//                 return NotFound(new { mensaje = "Persona no encontrada" });
//             }

//             personaExistente.DocumentoIdentidad = dto.DocumentoIdentidad;
//             personaExistente.Nombres = dto.Nombres;
//             personaExistente.Apellidos = dto.Apellidos;
//             personaExistente.Email = dto.Email;
//             personaExistente.Telefono = dto.Telefono;
//             personaExistente.TelefonoAlternativo = dto.TelefonoAlternativo;
//             personaExistente.FechaNacimiento = dto.FechaNacimiento;
//             personaExistente.Genero = dto.Genero;
//             personaExistente.EstadoCivil = dto.EstadoCivil;
//             personaExistente.Direccion = dto.Direccion;
//             personaExistente.Ciudad = dto.Ciudad;
//             personaExistente.EstadoProvincia = dto.EstadoProvincia;
//             personaExistente.CodigoPostal = dto.CodigoPostal;
//             personaExistente.Pais = dto.Pais;
//             personaExistente.TipoPersona = dto.TipoPersona;
//             personaExistente.UsuarioId = dto.UsuarioId;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!PersonaExists(id)) return NotFound();
//                 throw;
//             }

//             return NoContent();
//         }

//         // DELETE: api/personas/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeletePersona(int id)
//         {
//             var existe = await _context.Personas.AnyAsync(p => p.Id == id);
//             if (!existe)
//             {
//                 return NotFound(new { mensaje = "Persona no encontrada" });
//             }

//             // Usamos FALSE explícito en la query nativa. MySQL interpreta FALSE como 0 para las columnas tinyint(1).
//             // Esto evita los problemas de conversión que sufre el tipo sbyte con EF Core.
//             var filasAfectadas = await _context.Database.ExecuteSqlInterpolatedAsync(
//                 $"UPDATE personas SET Activo = FALSE WHERE Id = {id}"
//             );

//             if (filasAfectadas == 0)
//             {
//                 return BadRequest(new { mensaje = "No se pudo desactivar el registro" });
//             }

//             return Ok(new { mensaje = "Persona desactivada correctamente" });
//         }

//         private bool PersonaExists(int id)
//         {
//             return _context.Personas.Any(e => e.Id == id);
//         }
//     }
// }




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

        // PATCH: api/personas/5/estado?activar=true
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoPersona(int id, [FromQuery] bool activar = false)
        {
            var existe = await _context.Personas.AnyAsync(p => p.Id == id);
            if (!existe)
            {
                return NotFound(new { mensaje = "Persona no encontrada" });
            }

            // Usamos el casteo explícito a sbyte (1 o 0) según el parámetro recibido.
            // Esto asegura total compatibilidad con tipos sbyte? y columnas tinyint en MySQL.
            sbyte nuevoEstado = activar ? (sbyte)1 : (sbyte)0;

            var filasAfectadas = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE personas SET Activo = {nuevoEstado} WHERE Id = {id}"
            );

            if (filasAfectadas == 0)
            {
                return BadRequest(new { mensaje = "No se pudo modificar el estado del miembro" });
            }

            string accionRealizada = activar ? "activada" : "desactivada";
            return Ok(new { mensaje = $"Persona {accionRealizada} correctamente" });
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.Id == id);
        }
    }
}