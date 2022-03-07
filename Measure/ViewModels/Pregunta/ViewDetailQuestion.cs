using System.ComponentModel;

namespace Measure.ViewModels.Pregunta
{
    public class ViewDetailQuestion
    {
        [DisplayName("Tipo de Control")]
        public string TipoControl { get; set; }

        public string Texto { get; set; }

        public string Padre { get; set; }

        [DisplayName("Valor pregunta anidada")]
        public string ValorPadre { get; set; }

        public string Idioma { get; set; }

        public string Estado { get; set; }
    }
}