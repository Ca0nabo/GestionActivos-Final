using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    public class ProcesosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProcesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Pantalla para elegir Mes y Año
        public IActionResult Index()
        {
            return View();
        }

        // 2. La Lógica del Cálculo (El Cerebro)
        [HttpPost]
        public async Task<IActionResult> EjecutarDepreciacion(int anio, int mes)
        {
            // Validar que no se haya corrido ya ese mes
            var existe = await _context.CalculoDepreciacion
                .AnyAsync(c => c.AnoProceso == anio && c.MesProceso == mes);

            if (existe)
            {
                ViewBag.Mensaje = "Error: La depreciación para este período ya fue procesada.";
                ViewBag.Tipo = "danger";
                return View("Index");
            }

            // Buscar todos los activos que no estén depreciados totalmente
            // (Asumiremos una vida útil de 5 años = 60 meses para simplificar el ejemplo académico)
            var activos = await _context.ActivosFijos.Include(a => a.TipoActivo).ToListAsync();

            int registrosProcesados = 0;

            foreach (var activo in activos)
            {
                // Fórmula simple: Valor Compra / 60 meses (5 años)
                decimal montoMensual = activo.ValorCompra / 60;

                // Crear el registro de historial
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

                // Actualizar el acumulado en el activo
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