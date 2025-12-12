using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Authorize]
    public class ProcesosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProcesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EjecutarDepreciacion(int anio, int mes)
        {
            var existe = await _context.CalculoDepreciacion
                .AnyAsync(c => c.AnoProceso == anio && c.MesProceso == mes);

            if (existe)
            {
                ViewBag.Mensaje = "Error: La depreciación para este período ya fue procesada.";
                ViewBag.Tipo = "danger";
                return View("Index");
            }

            var activos = await _context.ActivosFijos.Include(a => a.TipoActivo).ToListAsync();

            int registrosProcesados = 0;

            foreach (var activo in activos)
            {
                decimal montoMensual = activo.ValorCompra / 60;

                var calculo = new CalculoDepreciacion
                {
                    AnoProceso = anio,
                    MesProceso = mes,
                    ActivoFijoId = activo.Id,
                    FechaProceso = DateTime.Now,
                    MontoDepreciado = montoMensual,
                    DepreciacionAcumulada = activo.DepreciacionAcumulada + montoMensual,
                    CuentaCompra = activo.TipoActivo?.CuentaCompra ?? "Sin Cuenta",
                    CuentaDepreciacion = activo.TipoActivo?.CuentaDepreciacion ?? "Sin Cuenta"
                };

                activo.DepreciacionAcumulada += montoMensual;

                _context.CalculoDepreciacion.Add(calculo);
                _context.ActivosFijos.Update(activo);
                registrosProcesados++;
            }

            await _context.SaveChangesAsync();

            ViewBag.Mensaje = $"Proceso Exitoso. Se calcularon {registrosProcesados} activos.";
            ViewBag.Tipo = "success";
            return View("Index");
        }
    }
}