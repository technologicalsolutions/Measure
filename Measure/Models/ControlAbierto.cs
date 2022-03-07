namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ControlAbierto")]
    public partial class ControlAbierto
    {
        public Guid Id { get; set; }

        [StringLength(1000)]
        public string DescripcionPre { get; set; }

        public int TipoDato { get; set; }

        [StringLength(1000)]
        public string DescripcionPos { get; set; }

        public bool? Antes { get; set; }

        public bool Multilinea { get; set; }

        public bool MultiCampo { get; set; }

        public int? TipoAbierta { get; set; }
    }
}
