using ControlCalidadProduccion.Data;
using ControlCalidadProduccion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ControlCalidadProduccion.Controllers
{
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Graficos(int? loteId, int? productoId)
        {
            // Obtener todos los lotes disponibles
            var lotes = _context.Lotes
                .OrderByDescending(l => l.Fecha)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.Codigo} - {l.Fecha:dd/MM/yyyy}"
                })
                .ToList();

            // Obtener todos los productos disponibles
            var productos = _context.Productos
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToList();

            // Pasar los datos a la vista
            ViewBag.Lotes = lotes;
            ViewBag.Productos = productos;
            ViewBag.LoteId = loteId;
            ViewBag.ProductoId = productoId;

            // Si no se seleccionaron ambos filtros, retornar vista vacía
            if (!loteId.HasValue || !productoId.HasValue)
            {
                return View(Enumerable.Empty<Medicion>());
            }

            // Consulta para obtener las mediciones filtradas con ordenamiento correcto
            var mediciones = _context.Mediciones
                .Include(m => m.Producto)
                .Include(m => m.Lote)
                .Where(m => m.LoteId == loteId.Value && m.ProductoId == productoId.Value)
                .AsEnumerable() // Cambiamos a evaluación en memoria
                .OrderBy(m => m.NumeroMedicion)
                .ThenBy(m => m.FechaMedicion) // Orden adicional por fecha
                .ToList();

            return View(mediciones);
        }
    }
}