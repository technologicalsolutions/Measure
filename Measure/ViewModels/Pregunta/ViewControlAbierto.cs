using System;

namespace Measure.ViewModels.Pregunta
{
    public class ViewControlAbierto
    {
        public Guid ControlId { get; set; }

        public Guid PreguntaId { get; set; }

        public Guid ClienteId { get; set; }

        public string TextoPregunta { get; set; }

        public int Idioma { get; set; }

        public string DescripcionPre { get; set; }

        public int TipoDato { get; set; }

        public string DescripcionPos { get; set; }

        public bool? Antes { get; set; }

        public bool Multilinea { get; set; }

        public bool MultiCampo { get; set; }

        public bool EstadoPregunta { get; set; }

        public bool Confirmacion { get; set; }

        public int TipoAbierta { get; set; }
    }
}