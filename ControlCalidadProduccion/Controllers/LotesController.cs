using ControlCalidadProduccion.Data;
using ControlCalidadProduccion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ControlCalidadProduccion.Controllers
{
    public class LotesController : Controller
    {
        private readonly AppDbContext _context;

        public LotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lotes
        public async Task<IActionResult> Index()
        {
            // Ordenar por ID descendente (los más nuevos primero)
            var lotes = await _context.Lotes
                .OrderByDescending(l => l.Id)
                .ToListAsync();

            return View(lotes);
        }

        // GET: Lotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lote == null)
            {
                return NotFound();
            }

            return View(lote);
        }

        // GET: Lotes/Create
        public IActionResult Create()
        {
            return View(new Lote { Fecha = DateTime.Today });
        }

        // POST: Lotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Fecha,Observaciones")] Lote lote)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(lote);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Lote creado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear lote: {ex.InnerException?.Message ?? ex.Message}";
                }
            }
            return View(lote);
        }

        // GET: Lotes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
            {
                return NotFound();
            }
            return View(lote);
        }

        // POST: Lotes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Fecha,Observaciones,FechaCreacion")] Lote lote)
        {
            if (id != lote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingLote = await _context.Lotes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                    if (existingLote != null)
                    {
                        lote.FechaCreacion = existingLote.FechaCreacion;
                    }

                    _context.Update(lote);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Lote actualizado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoteExists(lote.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(lote);
        }

        // GET: Lotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lote == null)
            {
                return NotFound();
            }

            return View(lote);
        }

        // POST: Lotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
            {
                return NotFound();
            }

            try
            {
                _context.Lotes.Remove(lote);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lote eliminado correctamente";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoteExists(int id)
        {
            return _context.Lotes.Any(e => e.Id == id);
        }
    }
}