using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IglesiaGo.Data; // <--- ¡Esta línea soluciona el error de compilación!

namespace IglesiaGo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // El constructor recibe el contexto de la base de datos automáticamente
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Traemos la lista real de personas desde MySQL para que las métricas y la tabla tengan datos
            var miembros = await _context.Personas.ToListAsync();
            
            return View(miembros);
        }
    }
}