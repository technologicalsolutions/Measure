using Measure.Models;
using Measure.ViewModels.Usuario;
using Measure.ViewModels.Grupo;

namespace Measure.ViewModels.Respuesta
{
    public class ViewContentListAnswer
    {
        public Contenido Inicio { get; set; }
        public ViewUpdateUser ActulizarUsuario { get; set; }
        public ViewLoadGroupPoll Categoria { get; set; }
        public Contenido Fin { get; set; }
        public Contenido Grabar { get; set; }
    }
}