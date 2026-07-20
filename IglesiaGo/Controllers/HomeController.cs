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

    // PÁGINA DE INICIO: Lista las noticias (mapeadas a la tabla 'noticias')
    public IActionResult Index()
    {
        try
        {
            var noticias = _context.Noticias
                .OrderByDescending(n => n.Fecha)
                .ToList();

            return View(noticias);
        }
        catch (Exception)
        {
            // Retorna una lista vacía para evitar que la vista explote si hay error de mapeo
            return View(new List<Noticia>());
        }
    }

    public IActionResult QuienesSomos()
    {
        return View();
    }

    // ENSEÑANZAS: Lista las enseñanzas (mapeadas a la tabla 'enseñanzas')
    public IActionResult Ensenanzas()
    {
        try
        {
            var ensenanzas = _context.Enseñanzas
                .OrderByDescending(e => e.FechaPublicacion)
                .ToList();

            return View(ensenanzas);
        }
        catch (Exception)
        {
            return View(new List<Enseñanza>());
        }
    }

    public IActionResult Contacto()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensaje(Consulta consulta)
    {
        if (ModelState.IsValid)
        {
            try 
            {
                consulta.FechaCreacion = DateTime.Now; 
                consulta.Atendido = false;

                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                TempData["MensajeEnviado"] = "Gracias por escribirnos. Nos pondremos en contacto pronto.";
                return RedirectToAction("Contacto");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo guardar la consulta. Verifique la conexión con MySQL.");
            }
        }

        return View("Contacto", consulta);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Buscamos el usuario en la tabla 'Usuarios'
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (usuario != null)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Error", "Correo o contraseña incorrectos");
        }
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}