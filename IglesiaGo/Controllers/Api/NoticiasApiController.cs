using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IglesiaGo.Data;
using Microsoft.AspNetCore.Authorization;

namespace IglesiaGo.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NoticiasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/noticiasapi
        [HttpGet]
        public async Task<IActionResult> GetNoticias()
        {
            var noticias = await _context.Noticias
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();

            return Ok(noticias); // Esto devuelve el JSON que leerá Android
        }

        // GET: api/noticiasapi/5
        [HttpGet("{id}")]
        // [Authorize] // Solo si la app está logueada puede ver detalles específicos
        public async Task<IActionResult> GetNoticia(int id)
        {
            var noticia = await _context.Noticias.FindAsync(id);

            if (noticia == null) return NotFound();

            return Ok(noticia);
        }
    }
}