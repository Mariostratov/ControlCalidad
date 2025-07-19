using System;
using System.ComponentModel.DataAnnotations;

namespace ControlCalidadProduccion.Models
{
    public class Lote
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código de lote es requerido")]
        [Display(Name = "Código de Lote")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Campo solo para compatibilidad con BD
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}