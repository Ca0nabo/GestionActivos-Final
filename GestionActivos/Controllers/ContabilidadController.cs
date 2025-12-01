using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContabilidadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Contabilidad?anio=2025&mes=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsientoContable>>> GetAsientos(int anio, int mes)
        {
            // 1. Buscamos las depreciaciones de ese mes
            var depreciaciones = await _context.CalculoDepreciacion
                .Include(d => d.ActivoFijo)
                .ThenInclude(a => a.TipoActivo) // Necesitamos el tipo para saber la cuenta
                .Where(d => d.AnoProceso == anio && d.MesProceso == mes)
                .ToListAsync();

            if (!depreciaciones.Any())
            {
                return NotFound($"No se encontraron depreciaciones para el período {mes}/{anio}.");
            }

            var asientos = new List<AsientoContable>();
            int contador = 1;

            // 2. Convertimos cada depreciación en asientos contables (Partida Doble)
            foreach (var item in depreciaciones)
            {
                // MOVIMIENTO 1: DÉBITO (Gasto de Depreciación)
                asientos.Add(new AsientoContable
                {
                    IdentificadorAsiento = contador++,
                    Descripcion = $"Depreciación {item.ActivoFijo?.Descripcion}",
                    IdentificadorTipoInventario = item.ActivoFijo?.TipoActivoId ?? 0,
                    CuentaContable = item.CuentaDepreciacion, // Cuenta de Gasto
                    TipoMovimiento = "DB",
                    FechaAsiento = item.FechaProceso,
                    MontoAsiento = item.MontoDepreciado,
                    Estado = true
                });

                // MOVIMIENTO 2: CRÉDITO (Depreciación Acumulada)
                asientos.Add(new AsientoContable
                {
                    IdentificadorAsiento = contador++,
                    Descripcion = $"Deprec. Acumulada {item.ActivoFijo?.Descripcion}",
                    IdentificadorTipoInventario = item.ActivoFijo?.TipoActivoId ?? 0,
                    CuentaContable = "1200-ACUM", // O la cuenta que definas para el crédito
                    TipoMovimiento = "CR",
                    FechaAsiento = item.FechaProceso,
                    MontoAsiento = item.MontoDepreciado,
                    Estado = true
                });
            }

            // 3. Devolvemos el JSON
            return Ok(asientos);
        }
    }
}