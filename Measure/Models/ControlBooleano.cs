namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ControlBooleano")]
    public partial class ControlBooleano
    {
        public Guid Id { get; set; }

        public bool TipoValor { get; set; }

        public bool? RespuestaCorrecta { get; set; }
    }
}
