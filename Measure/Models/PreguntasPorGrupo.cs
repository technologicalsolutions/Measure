namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PreguntasPorGrupo")]
    public partial class PreguntasPorGrupo
    {
        public Guid Id { get; set; }

        public Guid GrupoId { get; set; }

        public Guid PreguntaId { get; set; }

        public int Orden { get; set; }

        public bool Estado { get; set; }
    }
}
