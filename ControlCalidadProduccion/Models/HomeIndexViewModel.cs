using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ControlCalidadProduccion.Models
{
    public class HomeIndexViewModel
    {
        public List<SelectListItem> Productos { get; set; }
        public List<SelectListItem> Lotes { get; set; }
        public List<Medicion> UltimasMediciones { get; set; }
        public int ConteoLotes { get; set; }
        public int ConteoProductos { get; set; }
        public int ConteoMediciones { get; set; }
    }
}
