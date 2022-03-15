namespace Measure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MaestrasDetalle")]
    public partial class MaestrasDetalle
    {
        public int Id { get; set; }

        public int MaestraId { get; set; }

        [Required]
        [StringLength(255)]
        public string es_ES { get; set; }

        [Required]
        [StringLength(255)]
        public string en_US { get; set; }

        [Required]
        [StringLength(255)]
        public string pt_BR { get; set; }

        [Required]
        [StringLength(255)]
        public string Valor { get; set; }

        public bool Estado { get; set; }

        public virtual Maestras Maestras { get; set; }
    }
}
