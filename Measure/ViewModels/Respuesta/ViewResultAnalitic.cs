using System;

namespace Measure.ViewModels.Respuesta
{
    public class ViewResultAnalitic
    {
        public Guid IdAsignacion { get; set; }
        public string Correo { get; set; }
        public int OrdenGrupo { get; set; }
        public string Grupo { get; set; }
        public int OrdenPregunta { get; set; }
        public string Pregunta { get; set; }
        public int Respuesta { get; set; }
    }
}