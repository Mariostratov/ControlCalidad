using ControlCalidadProduccion.Data;
using ControlCalidadProduccion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlCalidadProduccion.Controllers
{
    public class MedicionesController : Controller
    {
        private readonly AppDbContext _context;

        public MedicionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Mediciones
        public async Task<IActionResult> Index(int? productoId, int? loteId)
        {
            var query = _context.Mediciones
                .Include(m => m.Lote)
                .Include(m => m.Producto)
                .AsQueryable();

            if (productoId.HasValue)
                query = query.Where(m => m.ProductoId == productoId.Value);

            if (loteId.HasValue)
                query = query.Where(m => m.LoteId == loteId.Value);

            var mediciones = await query.OrderByDescending(m => m.FechaMedicion).ToListAsync();

            await CargarListasDesplegables(productoId, loteId);

            return View(mediciones);
        }

        // GET: Mediciones/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasDesplegables();
            return View(new Medicion { FechaMedicion = DateTime.Now });
        }

        // POST: Mediciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicion medicion)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);
                return View(medicion);
            }

            var loteExiste = await _context.Lotes.AnyAsync(l => l.Id == medicion.LoteId);
            var productoExiste = await _context.Productos.AnyAsync(p => p.Id == medicion.ProductoId);

            if (!loteExiste || !productoExiste)
            {
                ModelState.AddModelError("", "El lote o producto seleccionado no existe.");
                await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);
                return View(medicion);
            }

            bool existeMedicion = await _context.Mediciones.AnyAsync(m =>
                m.LoteId == medicion.LoteId &&
                m.ProductoId == medicion.ProductoId &&
                m.NumeroMedicion == medicion.NumeroMedicion);

            if (existeMedicion)
            {
                ModelState.AddModelError("NumeroMedicion", "Ya existe una medición con ese número para el lote y producto seleccionados.");
                await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);
                return View(medicion);
            }

            try
            {
                _context.Add(medicion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medición registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al guardar la medición. Intente nuevamente.";
                await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);
                return View(medicion);
            }
        }

        // GET: Mediciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var medicion = await _context.Mediciones.FindAsync(id);
            if (medicion == null)
                return NotFound();

            await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);

            return View(medicion);
        }

        // POST: Mediciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicion medicion)
        {
            if (id != medicion.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarListasDesplegables(medicion.ProductoId, medicion.LoteId);
                return View(medicion);
            }

            try
            {
                _context.Update(medicion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medición actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Mediciones.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }
        }

        // GET: Mediciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var medicion = await _context.Mediciones
                .Include(m => m.Lote)
                .Include(m => m.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicion == null)
                return NotFound();

            return View(medicion);
        }

        // GET: Mediciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicion = await _context.Mediciones
                .Include(m => m.Lote)
                .Include(m => m.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicion == null)
            {
                return NotFound();
            }

            return View(medicion);
        }

        // POST: Mediciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicion = await _context.Mediciones.FindAsync(id);
            if (medicion == null)
            {
                return NotFound();
            }

            try
            {
                _context.Mediciones.Remove(medicion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medición eliminada correctamente.";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la medición: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        // Obtener siguiente número de medición
        [HttpGet]
        public async Task<JsonResult> ObtenerNumeroMedicion(int loteId, int productoId)
        {
            if (loteId <= 0 || productoId <= 0)
                return Json(1);

            int maxNumero = await _context.Mediciones
                .Where(m => m.LoteId == loteId && m.ProductoId == productoId)
                .MaxAsync(m => (int?)m.NumeroMedicion) ?? 0;

            int siguienteNumero = maxNumero + 1;
            if (siguienteNumero > 15)
                siguienteNumero = 15;

            return Json(siguienteNumero);
        }

        // Obtener lotes filtrados por producto
        [HttpGet]
        public async Task<JsonResult> ObtenerLotesPorProducto(int productoId)
        {
            if (productoId <= 0)
                return Json(new List<object>());

            var lotes = await _context.Lotes
                .Where(l => _context.Mediciones.Any(m => m.LoteId == l.Id && m.ProductoId == productoId))
                .OrderByDescending(l => l.Id) // Ordenar por ID descendente
                .Select(l => new { id = l.Id, nombre = l.Codigo }) // Solo código, sin ID
                .ToListAsync();

            return Json(lotes);
        }

        // Cargar listas para selects
        private async Task CargarListasDesplegables(int? productoId = null, int? loteId = null)
        {
            // Productos ordenados alfabéticamente
            ViewData["ProductoId"] = new SelectList(
                await _context.Productos
                    .OrderBy(p => p.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                productoId);

            // Lotes ordenados por ID descendente (más recientes primero)
            IQueryable<Lote> lotesQuery = _context.Lotes;

            if (productoId.HasValue)
            {
                lotesQuery = lotesQuery
                    .Where(l => _context.Mediciones.Any(m => m.LoteId == l.Id && m.ProductoId == productoId.Value));
            }

            var lotesList = await lotesQuery
                .OrderByDescending(l => l.Id) // Ordenar por ID descendente
                .ToListAsync();

            ViewData["LoteId"] = new SelectList(lotesList, "Id", "Codigo", loteId); // Solo muestra el código
        }
    }
}