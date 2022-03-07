namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Encuesta")]
    public partial class Encuesta
    {
        public Guid id { get; set; }

        public Guid ClienteId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Proposito { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool ActualizaUsuario { get; set; }

        public bool Estado { get; set; }
    }
}
