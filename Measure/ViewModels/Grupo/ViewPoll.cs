using System;

namespace Measure.ViewModels.Grupo
{
    public class ViewPoll
    {
        public Guid? id { get; set; }

        public Guid? ClienteId { get; set; }

        public string NombreCliente { get; set; }

        public string Nombre { get; set; }

        public string Proposito { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        public bool Estado { get; set; }

        public string EstadoFinalizado { get; set; }

        public bool ActualizaUsuario { get; set; }

        public bool Finalizada { get; set; }

        public Guid IdAsignacion { get; set; }

        public Guid TipoReporteGeneral { get; set; }
    }
}