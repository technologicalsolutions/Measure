namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ControlMatrizFila")]
    public partial class ControlMatrizFila
    {
        public Guid Id { get; set; }

        public Guid MatrizId { get; set; }

        [Required]
        public string Texto { get; set; }

        public int Orden { get; set; }

        public virtual ControlMatriz ControlMatriz { get; set; }
    }
}
