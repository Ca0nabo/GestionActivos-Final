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

        public async Task<IActionResult> Index(int? anio, int? mes)
        {
            var query = _context.CalculoDepreciacion.Include(c => c.ActivoFijo).AsQueryable();

            if (anio.HasValue)
            {
                query = query.Where(c => c.AnoProceso == anio.Value);
            }

            if (mes.HasValue)
            {
                query = query.Where(c => c.MesProceso == mes.Value);
            }

            var resultados = await query.ToListAsync();

            ViewBag.Anio = anio;
            ViewBag.Mes = mes;

            return View(resultados);
        }
    }
}