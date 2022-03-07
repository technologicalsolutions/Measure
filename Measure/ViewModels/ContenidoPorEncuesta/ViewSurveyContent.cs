using System;

namespace Measure.ViewModels.ContenidoPorEncuesta
{
    public class ViewSurveyContent
    {
        public Guid Id { get; set; }

        public Guid EncuestaId { get; set; }

        public Guid ClienteId { get; set; }

        public int TipoComponente { get; set; }

        public string TipoDeComponente { get; set; }

        public Guid ComponenteId { get; set; }

        public string NombreComponente { get; set; }
       
        public int Orden { get; set; }

        public bool Estado { get; set; }
    }
}