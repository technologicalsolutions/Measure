namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ControlMatriz")]
    public partial class ControlMatriz
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ControlMatriz()
        {
            ControlMatrizColumna = new HashSet<ControlMatrizColumna>();
            ControlMatrizFila = new HashSet<ControlMatrizFila>();
        }

        public Guid Id { get; set; }

        public int TipoControl { get; set; }

        public int Step { get; set; }

        public int? Minimo { get; set; }

        public int? Maximo { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ControlMatrizColumna> ControlMatrizColumna { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ControlMatrizFila> ControlMatrizFila { get; set; }
    }
}
