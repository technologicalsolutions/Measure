using System;

namespace Measure.ViewModels.Pregunta
{
    public class ViewControlBool
    {
        public Guid ControlId { get; set; }

        public Guid PreguntaId { get; set; }

        public Guid ClienteId { get; set; }

        public string TextoPregunta { get; set; }

        public int Idioma { get; set; }

        public bool EstadoPregunta { get; set; }

        public bool TipoValor { get; set; }        

        public bool? RespuestaCorrecta { get; set; }        
    }
}