using Measure.ViewModels.Usuario;
using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewCaptureResponse
    {
        public string IdUsuario { get; set; }
        public string IdEncuesta { get; set; }
        public string IdAsignacion { get; set; }
        public ViewUpdateUser Update { get; set; }
        public List<ViewAnswer> Respuestas { get; set; }
    }
}