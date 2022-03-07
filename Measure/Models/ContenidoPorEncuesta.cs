namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ContenidoPorEncuesta")]
    public partial class ContenidoPorEncuesta
    {
        public Guid Id { get; set; }

        public Guid EncuestaId { get; set; }

        public int TipoComponente { get; set; }

        public Guid ComponenteId { get; set; }

        public int Orden { get; set; }

        public bool Estado { get; set; }
    }
}
