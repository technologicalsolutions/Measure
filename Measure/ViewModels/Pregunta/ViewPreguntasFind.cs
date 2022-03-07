using System;

namespace Measure.ViewModels.Pregunta
{
    public class ViewPreguntasFind
    {
        public bool VisibleCliente { get; set; }
        public Guid? ClienteId { get; set; }        
        public int? Idioma { get; set; }
        public int? TipoControl { get; set; }
        public Guid? ControlId { get; set; }
        public Guid? ChildControlId { get; set; }
    }
}