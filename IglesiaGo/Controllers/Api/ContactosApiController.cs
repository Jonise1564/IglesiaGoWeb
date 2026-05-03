// using Microsoft.AspNetCore.Mvc;
// using IglesiaGo.Data;
// using IglesiaGo.Models;
// using Microsoft.EntityFrameworkCore;

// namespace IglesiaGo.Controllers.Api 
// {
//     [ApiController]
//     [Route("api/[controller]")] // La ruta será: api/ContactoApi
//     public class ContactoApiController : ControllerBase
//     {
//         private readonly ApplicationDbContext _context;

//         public ContactoApiController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         // GET: api/ContactoApi
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<Contacto>>> GetConsultas()
//         {
//             try
//             {
//                 var consultas = await _context.Contacto
//                     .OrderByDescending(c => c.FechaEnvio)
//                     .ToListAsync();

//                 return Ok(consultas);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new { mensaje = "Error al recuperar datos", error = ex.Message });
//             }
//         }

//         // GET: api/ContactoApi/5
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Contacto>> GetConsulta(int id)
//         {
//             var contacto = await _context.Contacto.FindAsync(id);

//             if (contacto == null) return NotFound();

//             return contacto;
//         }
//     }
// }



using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Data;
using IglesiaGo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // <--- Necesario para el bloqueo

namespace IglesiaGo.Controllers.Api 
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <--- Este es el "candado". Ahora pide Token para entrar.
    public class ContactoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ContactoApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contacto>>> GetConsultas()
        {
            try
            {
                var consultas = await _context.Contacto
                    .OrderByDescending(c => c.FechaEnvio)
                    .ToListAsync();

                return Ok(consultas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al recuperar Mensajes de consultas", error = ex.Message });
            }
        }

        // GET: api/ContactoApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contacto>> GetConsulta(int id)
        {
            var contacto = await _context.Contacto.FindAsync(id);

            if (contacto == null) return NotFound();

            return contacto;
        }

        // DELETE: api/ContactoApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsulta(int id)
        {
            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto == null) return NotFound();

            _context.Contacto.Remove(contacto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Eliminado correctamente" });
        }
    }
}