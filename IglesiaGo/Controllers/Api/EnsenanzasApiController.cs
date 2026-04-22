using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IglesiaGo.Data;

namespace IglesiaGo.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnsenanzasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnsenanzasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EnsenanzasApi
        [HttpGet]
        public async Task<IActionResult> GetEnsenanzas()
        {
            // Traemos todas las enseñanzas de la DB de Laragon
            var ensenanzas = await _context.Enseñanzas
                .OrderByDescending(e => e.FechaPublicacion)
                .ToListAsync();

            return Ok(ensenanzas);
        }
    }
}