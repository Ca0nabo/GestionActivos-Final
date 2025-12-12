using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Authorize]
    public class TipoActivosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoActivosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoActivos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposActivos.ToListAsync());
        }

        // GET: TipoActivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActivo = await _context.TiposActivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoActivo == null)
            {
                return NotFound();
            }

            return View(tipoActivo);
        }

        // GET: TipoActivos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoActivos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,CuentaCompra,CuentaDepreciacion,Estado")] TipoActivo tipoActivo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoActivo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoActivo);
        }

        // GET: TipoActivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActivo = await _context.TiposActivos.FindAsync(id);
            if (tipoActivo == null)
            {
                return NotFound();
            }
            return View(tipoActivo);
        }

        // POST: TipoActivos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,CuentaCompra,CuentaDepreciacion,Estado")] TipoActivo tipoActivo)
        {
            if (id != tipoActivo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoActivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoActivoExists(tipoActivo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoActivo);
        }

        // GET: TipoActivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoActivo = await _context.TiposActivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoActivo == null)
            {
                return NotFound();
            }

            return View(tipoActivo);
        }

        // POST: TipoActivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoActivo = await _context.TiposActivos.FindAsync(id);
            if (tipoActivo != null)
            {
                _context.TiposActivos.Remove(tipoActivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoActivoExists(int id)
        {
            return _context.TiposActivos.Any(e => e.Id == id);
        }
    }
}