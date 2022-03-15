namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Maestras
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Maestras()
        {
            MaestrasDetalle = new HashSet<MaestrasDetalle>();
        }

        public int Id { get; set; }

        public Guid ClienteId { get; set; }

        [StringLength(255)]
        public string es_ES { get; set; }

        [StringLength(255)]
        public string en_US { get; set; }

        [StringLength(255)]
        public string pt_BR { get; set; }

        public bool Activo { get; set; }

        public bool Base { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaestrasDetalle> MaestrasDetalle { get; set; }
    }
}
