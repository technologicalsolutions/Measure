namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ControlMatrizColumna")]
    public partial class ControlMatrizColumna
    {
        public Guid Id { get; set; }

        public Guid MatrizId { get; set; }

        [Required]
        public string Texto { get; set; }

        public int PasosPorColumna { get; set; }

        public int Orden { get; set; }

        public virtual ControlMatriz ControlMatriz { get; set; }
    }
}
