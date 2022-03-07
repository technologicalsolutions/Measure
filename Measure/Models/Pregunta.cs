namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pregunta")]
    public partial class Pregunta
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        public int TipoControl { get; set; }

        public Guid ControlId { get; set; }

        [Required]
        public string Texto { get; set; }

        public int Idioma { get; set; }

        public bool Confirmacion { get; set; }

        public bool Estado { get; set; }
    }
}
