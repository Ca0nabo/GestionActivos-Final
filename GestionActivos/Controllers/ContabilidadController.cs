using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class ContabilidadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsientoContable>>> GetAsientos(int anio, int mes)
        {
            var depreciaciones = await _context.CalculoDepreciacion
                .Include(d => d.ActivoFijo)
                .ThenInclude(a => a.TipoActivo)
                .Where(d => d.AnoProceso == anio && d.MesProceso == mes)
                .ToListAsync();

            if (!depreciaciones.Any())
            {
                return NotFound($"No se encontraron depreciaciones para el período {mes}/{anio}.");
            }

            var asientos = new List<AsientoContable>();
            int contador = 1;

            foreach (var item in depreciaciones)
            {
                asientos.Add(new AsientoContable
                {
                    IdentificadorAsiento = contador++,
                    Descripcion = $"Depreciación {item.ActivoFijo?.Descripcion}",
                    IdentificadorTipoInventario = item.ActivoFijo?.TipoActivoId ?? 0,
                    CuentaContable = item.CuentaDepreciacion,
                    TipoMovimiento = "DB",
                    FechaAsiento = item.FechaProceso,
                    MontoAsiento = item.MontoDepreciado,
                    Estado = true
                });

                asientos.Add(new AsientoContable
                {
                    IdentificadorAsiento = contador++,
                    Descripcion = $"Deprec. Acumulada {item.ActivoFijo?.Descripcion}",
                    IdentificadorTipoInventario = item.ActivoFijo?.TipoActivoId ?? 0,
                    CuentaContable = "1200-ACUM",
                    TipoMovimiento = "CR",
                    FechaAsiento = item.FechaProceso,
                    MontoAsiento = item.MontoDepreciado,
                    Estado = true
                });
            }

            return Ok(asientos);
        }
    }
}