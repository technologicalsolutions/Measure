namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Reporte")]
    public class Reporte
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reporte()
        {
            ContenidosReporte = new HashSet<ContenidoReporte>();
        }

        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        [StringLength(250)]
        public string es_ES { get; set; }

        [StringLength(250)]
        public string en_US { get; set; }

        [StringLength(250)]
        public string pt_BR { get; set; }

        public bool Estado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContenidoReporte> ContenidosReporte { get; set; }
    }
}