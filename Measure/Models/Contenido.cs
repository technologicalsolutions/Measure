namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contenido")]
    public partial class Contenido
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        [StringLength(250)]
        public string es_Es { get; set; }

        [StringLength(250)]
        public string en_US { get; set; }

        [StringLength(250)]
        public string pt_BR { get; set; }

        public int? Idioma { get; set; }

        public bool Posicion { get; set; }

        public string Html { get; set; }

        public int TipoContenido { get; set; }

        public bool Estado { get; set; }
    }
}
