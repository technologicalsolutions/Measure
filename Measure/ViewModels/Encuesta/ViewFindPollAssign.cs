using System;

namespace Measure.ViewModels.Encuesta
{
    public class ViewFindPollAssign
    {
        public Guid ClienteId { get; set; }
        public string email { get; set; }
        public string Nombres { get; set; }
        public int? Idioma { get; set; }
        public int? CodigoPais { get; set; }
    }
}
