namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Grupo")]
    public partial class Grupo
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        [StringLength(250)]
        public string es_Es { get; set; }

        [StringLength(250)]
        public string en_US { get; set; }

        [StringLength(250)]
        public string pt_BR { get; set; }

        [StringLength(250)]
        public string Desc_es_Es { get; set; }

        [StringLength(250)]
        public string Desc_en_US { get; set; }

        [StringLength(250)]
        public string Desc_pt_BR { get; set; }

        public int? Columnas { get; set; }

        public int? TipoReporte { get; set; }

        public int? RespuestaBajaMax { get; set; }

        public int? RespuestaAltaMin { get; set; }

        [StringLength(20)]
        public string Color { get; set; }

        public bool Estado { get; set; }
    }
}
