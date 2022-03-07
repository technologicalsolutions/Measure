namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UsuariosPorEncuenta")]
    public partial class UsuariosPorEncuenta
    {
        public Guid Id { get; set; }

        public Guid EncuestaId { get; set; }

        public Guid UsuarioId { get; set; }        
    }
}
