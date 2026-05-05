using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Models;
using IglesiaGo.Data;
using Microsoft.EntityFrameworkCore;

namespace IglesiaGo.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // PÁGINA DE INICIO: Carga las noticias ordenadas por fecha
    public IActionResult Index()
    {
        var noticias = _context.Noticias
            .OrderByDescending(n => n.Fecha)
            .ToList();

        return View(noticias);
    }

    // QUIÉNES SOMOS: Vista estática institucional
    public IActionResult QuienesSomos()
    {
        return View();
    }

    // ENSEÑANZAS BÍBLICAS: Lista de enseñanzas desde la DB
    public IActionResult Ensenanzas()
    {
        var ensenanzas = _context.Enseñanzas
            .OrderByDescending(e => e.FechaPublicacion)
            .ToList();

        return View(ensenanzas);
    }

    // CONTACTO (GET): Muestra el formulario vacío
    public IActionResult Contacto()
    {
        return View();
    }

    // CONTACTO (POST): Procesa y guarda la nueva Consulta
    [HttpPost]
    public async Task<IActionResult> EnviarMensaje(Consulta consulta)
    {
        if (ModelState.IsValid)
        {
            try 
            {
                // Asignamos valores por defecto antes de guardar
                consulta.FechaCreacion = DateTime.Now; 
                consulta.Atendido = false;

                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                TempData["MensajeEnviado"] = "Gracias por escribirnos. Nos pondremos en contacto pronto.";
                return RedirectToAction("Contacto");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al intentar guardar en la base de datos.");
            }
        }

        // Si el modelo no es válido, regresamos a la vista con los errores
        return View("Contacto", consulta);
    }

    // LOGIN (GET): Muestra el formulario de acceso
    public IActionResult Login()
    {
        return View();
    }

    // LOGIN (POST): Valida las credenciales del usuario
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Búsqueda simple (Para producción considera usar Hash real de contraseñas)
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (usuario != null)
            {
                // Aquí podrías implementar el manejo de Cookies o JWT
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Error", "Correo o contraseña incorrectos");
        }
        return View(model);
    }

    // MANEJO DE ERRORES
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}