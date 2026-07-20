using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Data;
using IglesiaGo.Models;
using Microsoft.EntityFrameworkCore;

namespace IglesiaGo.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Acciones de cara al Usuario (App/Web) ---

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarMensaje([Bind("NombreRemitente,Telefono,mail,Mensaje")] Consulta consulta)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", consulta);
            }

            try
            {
                // FechaCreacion tiene DEFAULT CURRENT_TIMESTAMP en MySQL, pero podemos setearla igual
                consulta.FechaCreacion = DateTime.Now;
                consulta.Atendido = false; 
                
                _context.Add(consulta);
                await _context.SaveChangesAsync();

                TempData["Success"] = "¡Mensaje enviado con éxito!";
                return RedirectToAction(nameof(Index), new { enviado = true });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error técnico al procesar tu solicitud.");
                return View("Index", consulta);
            }
        }

        // --- Acciones de Administración (Backend) ---

        public async Task<IActionResult> Listado()
        {
            // Ordenamos para que las consultas no atendidas aparezcan primero
            var listado = await _context.Consultas
                .OrderBy(c => c.Atendido)
                .ThenByDescending(c => c.FechaCreacion)
                .ToListAsync();
            return View(listado);
        }

        // GET: Muestra el hilo de la conversación y el formulario para responder
        public async Task<IActionResult> Responder(int? id)
        {
            if (id == null) return NotFound();

            // Incluimos los mensajes relacionados para ver el historial en la vista
            var consulta = await _context.Consultas
                .Include(c => c.Mensajes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null) return NotFound();

            return View(consulta);
        }

        // POST: Agrega un mensaje al hilo y marca como atendido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarRespuesta(int consultaId, string cuerpoMensaje)
        {
            if (string.IsNullOrWhiteSpace(cuerpoMensaje))
            {
                TempData["Error"] = "El mensaje de respuesta no puede estar vacío.";
                return RedirectToAction(nameof(Responder), new { id = consultaId });
            }

            var consulta = await _context.Consultas.FindAsync(consultaId);
            if (consulta == null) return NotFound();

            try
            {
                // 1. Creamos el nuevo registro en la tabla mensajes_consulta
                var respuesta = new MensajeConsulta
                {
                    ConsultaId = consultaId,
                    TipoRemitente = "Admin", // Importante para el diseño en la App
                    CuerpoMensaje = cuerpoMensaje,
                    FechaEnvio = DateTime.Now
                };

                _context.MensajesConsulta.Add(respuesta);

                // 2. Marcamos la consulta principal como atendida
                consulta.Atendido = true;
                _context.Update(consulta);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Respuesta enviada correctamente.";
                return RedirectToAction(nameof(Listado));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al guardar la respuesta.";
                return RedirectToAction(nameof(Responder), new { id = consultaId });
            }
        }
    }
}