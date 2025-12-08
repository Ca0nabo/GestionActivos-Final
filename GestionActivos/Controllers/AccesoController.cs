using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string clave)
        {
            // Busca en la BD
            Usuario? userEncontrado = _context.Usuarios
                .FirstOrDefault(u => u.Correo == usuario && u.Clave == clave);

            if (userEncontrado != null)
            {
                // Guarda los datos en la cookie (incluyendo el ROL)
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userEncontrado.Nombre),
                    new Claim(ClaimTypes.Email, userEncontrado.Correo),
                    new Claim(ClaimTypes.Role, userEncontrado.Rol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}