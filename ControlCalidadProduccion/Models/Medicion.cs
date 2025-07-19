using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlCalidadProduccion.Models
{
    public class Medicion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lote")]
        public int LoteId { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        // Quitar Required para que no de error al enviar
        [Display(Name = "Número de Medición")]
        public int NumeroMedicion { get; set; }

        [Required]
        [Display(Name = "Grasa (%)")]
        public decimal Grasa { get; set; }

        [Required]
        [Display(Name = "Acidez")]
        public decimal Acidez { get; set; }

        [Required]
        [Display(Name = "Proteína")]
        public decimal Proteina { get; set; }

        [Required]
        [Display(Name = "pH")]
        public decimal PH { get; set; }

        [Required]
        [Display(Name = "Humedad (%)")]
        public decimal Humedad { get; set; }

        [Required]
        [Display(Name = "Fecha de Medición")]
        [DataType(DataType.DateTime)]
        public DateTime FechaMedicion { get; set; } = DateTime.Now;

        // Relaciones de navegación (opcional)
        [ForeignKey("LoteId")]
        public virtual Lote? Lote { get; set; }

        [ForeignKey("ProductoId")]
        public virtual Producto? Producto { get; set; }
    }
}