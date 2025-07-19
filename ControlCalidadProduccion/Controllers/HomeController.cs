using ControlCalidadProduccion.Data;
using ControlCalidadProduccion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlCalidadProduccion.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? productoId, int? loteId)
        {
            // Obtener listas de productos y lotes
            var productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToListAsync();

            List<SelectListItem> lotes;
            if (productoId.HasValue)
            {
                lotes = await _context.Lotes
                    .Where(l => _context.Mediciones.Any(m => m.LoteId == l.Id && m.ProductoId == productoId.Value))
                    .OrderByDescending(l => l.Fecha)
                    .Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Codigo })
                    .ToListAsync();
            }
            else
            {
                lotes = new List<SelectListItem>();
            }

            // Obtener últimas mediciones
            var ultimasMedicionesQuery = _context.Mediciones
                .Include(m => m.Lote)
                .Include(m => m.Producto)
                .OrderByDescending(m => m.FechaMedicion)
                .AsQueryable();

            if (productoId.HasValue)
                ultimasMedicionesQuery = ultimasMedicionesQuery.Where(m => m.ProductoId == productoId.Value);

            if (loteId.HasValue)
                ultimasMedicionesQuery = ultimasMedicionesQuery.Where(m => m.LoteId == loteId.Value);

            var ultimasMediciones = await ultimasMedicionesQuery.Take(10).ToListAsync();

            // Obtener datos para gráficos si hay selección
            IEnumerable<Medicion> medicionesGrafico = new List<Medicion>();
            var primeraMedicion = new Medicion();

            if (productoId.HasValue && loteId.HasValue)
            {
                medicionesGrafico = await _context.Mediciones
                    .Include(m => m.Lote)
                    .Include(m => m.Producto)
                    .Where(m => m.ProductoId == productoId.Value && m.LoteId == loteId.Value)
                    .OrderBy(m => m.NumeroMedicion)
                    .ToListAsync();

                primeraMedicion = medicionesGrafico.FirstOrDefault();
            }

            // Estadísticas generales
            var conteoLotes = await _context.Lotes.CountAsync();
            var conteoProductos = await _context.Productos.CountAsync();
            var conteoMediciones = await _context.Mediciones.CountAsync();

            // ViewBags
            ViewBag.Productos = productos;
            ViewBag.Lotes = lotes;
            ViewBag.UltimasMediciones = ultimasMediciones;
            ViewBag.ConteoLotes = conteoLotes;
            ViewBag.ConteoProductos = conteoProductos;
            ViewBag.ConteoMediciones = conteoMediciones;
            ViewBag.ProductoIdSeleccionado = productoId?.ToString() ?? "";
            ViewBag.LoteIdSeleccionado = loteId?.ToString() ?? "";
            ViewBag.Mediciones = medicionesGrafico;
            ViewBag.PrimeraMedicion = primeraMedicion;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerLotesPorProducto(int productoId)
        {
            var lotes = await _context.Lotes
                .Where(l => _context.Mediciones.Any(m => m.LoteId == l.Id && m.ProductoId == productoId))
                .OrderByDescending(l => l.Fecha)
                .Select(l => new { id = l.Id, nombre = l.Codigo })
                .ToListAsync();

            return Json(lotes);
        }
    }
}