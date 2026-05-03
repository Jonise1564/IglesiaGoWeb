using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Data;
using IglesiaGo.Models;
using Microsoft.EntityFrameworkCore;

namespace IglesiaGo.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarMensaje([Bind("NombreRemitente,Telefono,mail,Mensaje")] Contacto contacto)
        {
            // Esto chequea automáticamente las Data Annotations del modelo
            if (!ModelState.IsValid)
            {
                // Si hay error, volvemos a la vista mostrando qué falló
                return View("Index", contacto);
            }

            try
            {
                contacto.FechaEnvio = DateTime.Now;
                _context.Add(contacto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "¡Mensaje enviado con éxito!";
                return RedirectToAction(nameof(Index), new { enviado = true });
            }
            catch (Exception ex)
            {
                // Loguear el error (ex) sería lo ideal aquí
                ModelState.AddModelError("", "Ocurrió un error técnico al procesar tu solicitud.");
                return View("Index", contacto);
            }
        }
        public async Task<IActionResult> Listado()
        {
            var listado = await _context.Contacto.OrderByDescending(c => c.FechaEnvio).ToListAsync();
            return View(listado);
        }
    }
}