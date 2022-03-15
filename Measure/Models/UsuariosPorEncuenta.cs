namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UsuariosPorEncuenta")]
    public partial class UsuariosPorEncuenta
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EncuestaId { get; set; }

        public Guid UsuarioId { get; set; }

        public bool Resuelta { get; set; }

        public DateTime? FechaResuelta { get; set; }
    }
}
