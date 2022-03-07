using System;

namespace Measure.ViewModels.Pregunta
{
    public class ViewControlMatriz
    {
        public Guid ControlId { get; set; }

        public Guid PreguntaId { get; set; }

        public Guid ClienteId { get; set; }

        public string TextoPregunta { get; set; }

        public int Idioma { get; set; }

        public bool EstadoPregunta { get; set; }

        public string TextoFilas { get; set; }

        public string TextoColumnas { get; set; }

        public Enums.ControlMatriz Radio { get; set; }

        public int? Min { get; set; }

        public int? Max { get; set; }

        public int? Step { get; set; }

        public string PasosPorColumna { get; set; }
        
    }
}