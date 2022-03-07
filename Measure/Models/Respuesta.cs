namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Respuesta")]
    public partial class Respuesta
    {
        public Guid Id { get; set; }

        public Guid IdAsignacion { get; set; }

        public Guid PreguntaId { get; set; }

        public Guid UsuarioId { get; set; }

        public int TipoControl { get; set; }

        public string Resultado { get; set; }

        public DateTime? FechaRespuesta { get; set; }
    }
}
