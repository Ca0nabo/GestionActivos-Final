using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionActivos.Data;
using GestionActivos.Models;

namespace GestionActivos.Controllers
{
    [Authorize]
    public class ActivoFijosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivoFijosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActivoFijos
        public async Task<IActionResult> Index()
        {
            // Incluimos AMBAS relaciones
            var activosCompletos = _context.ActivosFijos
                .Include(a => a.Departamento)
                .Include(a => a.TipoActivo);

            return View(await activosCompletos.ToListAsync());
        }

        // GET: ActivoFijos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoFijo = await _context.ActivosFijos
                .Include(a => a.Departamento)
                .Include(a => a.TipoActivo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activoFijo == null)
            {
                return NotFound();
            }

            return View(activoFijo);
        }

        // GET: ActivoFijos/Create
        public IActionResult Create()
        {
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Id");
            ViewData["TipoActivoId"] = new SelectList(_context.TiposActivos, "Id", "Id");
            return View();
        }

        // POST: ActivoFijos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,DepartamentoId,TipoActivoId,FechaRegistro,ValorCompra,DepreciacionAcumulada")] ActivoFijo activoFijo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activoFijo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Id", activoFijo.DepartamentoId);
            ViewData["TipoActivoId"] = new SelectList(_context.TiposActivos, "Id", "Id", activoFijo.TipoActivoId);
            return View(activoFijo);
        }

        // GET: ActivoFijos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoFijo = await _context.ActivosFijos.FindAsync(id);
            if (activoFijo == null)
            {
                return NotFound();
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Id", activoFijo.DepartamentoId);
            ViewData["TipoActivoId"] = new SelectList(_context.TiposActivos, "Id", "Id", activoFijo.TipoActivoId);
            return View(activoFijo);
        }

        // POST: ActivoFijos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,DepartamentoId,TipoActivoId,FechaRegistro,ValorCompra,DepreciacionAcumulada")] ActivoFijo activoFijo)
        {
            if (id != activoFijo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activoFijo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivoFijoExists(activoFijo.Id))
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
            ViewData["DepartamentoId"] = new SelectList(_context.Departamentos, "Id", "Id", activoFijo.DepartamentoId);
            ViewData["TipoActivoId"] = new SelectList(_context.TiposActivos, "Id", "Id", activoFijo.TipoActivoId);
            return View(activoFijo);
        }

        // GET: ActivoFijos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoFijo = await _context.ActivosFijos
                .Include(a => a.Departamento)
                .Include(a => a.TipoActivo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activoFijo == null)
            {
                return NotFound();
            }

            return View(activoFijo);
        }

        // POST: ActivoFijos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activoFijo = await _context.ActivosFijos.FindAsync(id);
            if (activoFijo != null)
            {
                _context.ActivosFijos.Remove(activoFijo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivoFijoExists(int id)
        {
            return _context.ActivosFijos.Any(e => e.Id == id);
        }
    }
}
