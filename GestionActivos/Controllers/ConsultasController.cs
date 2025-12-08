using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Pantalla de Busqueda
        public async Task<IActionResult> Index(int? anio, int? mes)
        {
            // Consultamos la tabla de Historial
            var query = _context.CalculoDepreciacion.Include(c => c.ActivoFijo).AsQueryable();

            // Si el usuario escribe un año, filtramos
            if (anio.HasValue)
            {
                query = query.Where(c => c.AnoProceso == anio.Value);
            }

            // Si el usuario elige un mes, filtramos
            if (mes.HasValue)
            {
                query = query.Where(c => c.MesProceso == mes.Value);
            }

            var resultados = await query.ToListAsync();

            // Para que los campos del filtro no se borren al buscar
            ViewBag.Anio = anio;
            ViewBag.Mes = mes;

            return View(resultados);
        }
    }
}