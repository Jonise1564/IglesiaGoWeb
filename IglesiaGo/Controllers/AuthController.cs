using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IglesiaGo.Data;
using IglesiaGo.Models;

namespace IglesiaGo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Validar el usuario contra la DB (MySQL en Laragon)
                // Nota: En producción, usa hashing para comparar contraseñas
                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (usuario != null)
                {
                    // 2. Generar el Token si los datos son correctos
                    var token = GenerarTokenJWT(usuario);

                    return Ok(new { token = token, message = "Login exitoso" });
                }

                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return BadRequest(ModelState);
        }

        private string GenerarTokenJWT(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UsuarioId", usuario.Id.ToString()),
                // Puedes agregar roles aquí si los tienes
                new Claim(ClaimTypes.Role, "Admin") 
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3), // El token vence en 3 horas
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}