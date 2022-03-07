using System;

namespace Measure.ViewModels.Pregunta
{
    public class ViewControlCerrado
    {
        public Guid ControlId { get; set; }

        public Guid PreguntaId { get; set; }

        public Guid ClienteId { get; set; }

        public string TextoPregunta { get; set; }

        public int Idioma { get; set; }

        public bool EstadoPregunta { get; set; }

        public int TipoCerrada { get; set; }

        public Guid? PadreId { get; set; }

        public string Textos { get; set; }

        public string Valores { get; set; }

        public bool? TieneControl { get; set; }

        public string ValorControl { get; set; }

        public int? TipoDato { get; set; }

        public bool? Radio { get; set; }

        public string RespuestaCorrecta { get; set; }        
    }
}