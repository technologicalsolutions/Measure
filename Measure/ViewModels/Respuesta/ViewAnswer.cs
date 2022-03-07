using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswer
    {
        public string Id { get; set; }
        public int TipoControl { get; set; }
        public string SubControl { get; set; }
        public List<string> Respuesta { get; set; }
    }
}