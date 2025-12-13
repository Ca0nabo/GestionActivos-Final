using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;
using GestionActivos.Services;
using GestionActivos.DTOs;

namespace GestionActivos.Controllers
{
    [Authorize]
    public class ProcesosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ContabilidadService _contabilidadService;

        public ProcesosController(ApplicationDbContext context, ContabilidadService contabilidadService)
        {
            _context = context;
            _contabilidadService = contabilidadService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EjecutarDepreciacion(int anio, int mes)
        {
            var existe = await _context.CalculoDepreciacion.AnyAsync(c => c.AnoProceso == anio && c.MesProceso == mes);
            if (existe)
            {
                ViewBag.Mensaje = "Error: Este período ya fue procesado.";
                ViewBag.Tipo = "danger";
                return View("Index");
            }

            var activos = await _context.ActivosFijos.Include(a => a.TipoActivo).ToListAsync();
            int procesados = 0;
            int enviados = 0;
            string detalleError = "";

            // 1. Buscamos una cuenta válida AUTOMÁTICAMENTE para no fallar
            int cuentaValidaId = 0;
            try
            {
                cuentaValidaId = await _contabilidadService.ObtenerCuentaIdValidaAsync();
            }
            catch (Exception ex)
            {
                detalleError = "No pude conectarme para buscar cuentas: " + ex.Message;
            }

            foreach (var activo in activos)
            {
                // A. Lógica de Negocio
                int vidaUtil = activo.TipoActivo?.VidaUtil > 0 ? activo.TipoActivo.VidaUtil : 60;
                decimal monto = activo.ValorCompra / vidaUtil;

                var calculo = new CalculoDepreciacion
                {
                    AnoProceso = anio,
                    MesProceso = mes,
                    ActivoFijoId = activo.Id,
                    FechaProceso = DateTime.Now,
                    MontoDepreciado = monto,
                    DepreciacionAcumulada = activo.DepreciacionAcumulada + monto,
                    CuentaCompra = activo.TipoActivo?.CuentaCompra ?? "0",
                    CuentaDepreciacion = activo.TipoActivo?.CuentaDepreciacion ?? "0"
                };
                activo.DepreciacionAcumulada += monto;
                _context.CalculoDepreciacion.Add(calculo);
                _context.ActivosFijos.Update(activo);

                // B. Integración con Contabilidad
                if (cuentaValidaId > 0) // Solo intentamos si tenemos una cuenta real
                {
                    try
                    {
                        var asientoDB = new AsientoRequest
                        {
                            description = $"Deprec. {activo.Descripcion}",
                            accountId = cuentaValidaId, // ¡USAMOS LA ID REAL QUE ENCONTRAMOS!
                            movementType = "DB",
                            amount = monto,
                            entryDate = DateTime.Now.ToString("yyyy-MM-dd")
                        };

                        var asientoCR = new AsientoRequest
                        {
                            description = $"Acumulada {activo.Descripcion}",
                            accountId = cuentaValidaId,
                            movementType = "CR",
                            amount = monto,
                            entryDate = DateTime.Now.ToString("yyyy-MM-dd")
                        };

                        await _contabilidadService.EnviarAsientoAsync(asientoDB);
                        await _contabilidadService.EnviarAsientoAsync(asientoCR);
                        enviados += 2;
                    }
                    catch (Exception ex)
                    {
                        detalleError = ex.Message;
                    }
                }

                procesados++;
            }

            await _context.SaveChangesAsync();

            if (enviados > 0)
            {
                ViewBag.Mensaje = $"✅ ÉXITO TOTAL. Activos: {procesados}. Asientos Enviados: {enviados}. (Usando cuenta ID: {cuentaValidaId})";
                ViewBag.Tipo = "success";
            }
            else
            {
                ViewBag.Mensaje = $"⚠️ Cierre Local OK, pero la integración falló. Error: {detalleError}";
                ViewBag.Tipo = "warning";
            }

            return View("Index");
        }
    }
}