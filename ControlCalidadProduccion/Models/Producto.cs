using System;
using System.ComponentModel.DataAnnotations;

namespace ControlCalidadProduccion.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Norma Grasa (%)")]
        [Range(0, 100)]
        public decimal NormaGrasa { get; set; }

        [Required]
        [Display(Name = "Norma Acidez (%)")]
        [Range(0, 100)]
        public decimal NormaAcidez { get; set; }

        [Required]
        [Display(Name = "Norma Proteína (%)")]
        [Range(0, 100)]
        public decimal NormaProteina { get; set; }

        [Required]
        [Display(Name = "Norma pH")]
        [Range(0, 14)]
        public decimal NormaPH { get; set; }

        [Required]
        [Display(Name = "Norma Humedad (%)")]
        [Range(0, 100)]
        public decimal NormaHumedad { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}