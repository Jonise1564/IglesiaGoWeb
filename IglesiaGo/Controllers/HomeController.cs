// using System.Diagnostics;
// using Microsoft.AspNetCore.Mvc;
// using IglesiaGo.Models;
// using IglesiaGo.Data; // Importante para acceder a la DB
// using System.Linq;

// namespace IglesiaGo.Controllers;

// public class HomeController : Controller
// {
//     private readonly ApplicationDbContext _context;

//     // El constructor recibe el contexto de la base de datos
//     public HomeController(ApplicationDbContext context)
//     {
//         _context = context;
//     }

//     public IActionResult Index()
//     {
//         // Traemos las últimas 3 noticias para el Home
//         var noticias = _context.Noticias
//             .OrderByDescending(n => n.Fecha)
//             .Take(3)
//             .ToList();

//         return View(noticias); // Pasamos la lista a la Vista
//     }

//     public IActionResult QuienesSomos()
//     {
//         return View();
//     }

//     public IActionResult Contacto()
//     {
//         return View();
//     }

//     [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//     public IActionResult Error()
//     {
//         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//     }
// }



using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IglesiaGo.Models;
using IglesiaGo.Data;
using System.Linq;

namespace IglesiaGo.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // PÁGINA DE INICIO: Carga las noticias
    public IActionResult Index()
    {
        var noticias = _context.Noticias
            .OrderByDescending(n => n.Fecha)
            // .Take(3)
            .ToList();

        return View(noticias);
    }

    // QUIÉNES SOMOS: Solo devuelve la vista estática
    public IActionResult QuienesSomos()
    {
        return View();
    }

    // ENSEÑANZAS BÍBLICAS: Carga la lista de enseñanzas de la DB
    public IActionResult Ensenanzas()
    {
        var ensenanzas = _context.Enseñanzas
            .OrderByDescending(e => e.FechaPublicacion)
            .ToList();

        return View(ensenanzas);
    }

    // CONTACTO (GET): Muestra el formulario
    public IActionResult Contacto()
    {
        return View();
    }

    // CONTACTO (POST): Recibe los datos del formulario y los guarda en Laragon
    [HttpPost]
    public async Task<IActionResult> EnviarMensaje(Contacto contacto)
    {
        if (ModelState.IsValid)
        {
            contacto.FechaEnvio = DateTime.Now; // Seteamos la fecha actual
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();

            TempData["MensajeEnviado"] = "Gracias por escribirnos. Nos pondremos en contacto pronto.";
            return RedirectToAction("Contacto");
        }

        return View("Contacto", contacto);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    // GET: Muestra la vista de Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Procesa el inicio de sesión
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Buscamos el usuario en la base de datos de Laragon
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (usuario != null)
            {
                // AQUÍ: En un sistema real usarías Claims y Cookies. 
                // Por ahora, simulamos el éxito:
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Error", "Correo o contraseña incorrectos");
        }
        return View(model);
    }


}