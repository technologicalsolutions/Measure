using Measure.ViewModels.Error;
using System;
using System.ComponentModel.DataAnnotations;

namespace Measure.ViewModels.Pregunta
{
    public class ViewPregunta : ViewModelError
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        public int TipoControl { get; set; }

        public Guid ControlId { get; set; }

        [Required]
        public string Texto { get; set; }

        public int Idioma { get; set; }

        public string IdiomaName { get; set; }

        public bool Confirmacion { get; set; }

        public bool Estado { get; set; }

        public string EstadoName { get; set; }
    }
}