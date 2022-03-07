namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ControlCerrado")]
    public partial class ControlCerrado
    {
        public Guid Id { get; set; }

        public int TipoCerrada { get; set; }

        public Guid? PadreId { get; set; }

        [Required]
        public string Textos { get; set; }

        [Required]
        public string Valores { get; set; }

        public bool? TieneControl { get; set; }

        [StringLength(3000)]
        public string ValorControl { get; set; }

        public int? TipoDato { get; set; }

        public bool? Radio { get; set; }

        [StringLength(1000)]
        public string RespuestaCorrecta { get; set; }
    }
}
