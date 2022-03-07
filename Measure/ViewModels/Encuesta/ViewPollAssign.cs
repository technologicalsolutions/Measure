using Measure.ViewModels.Usuario;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Encuesta
{
    public class ViewPollAssign
    {
        public Models.Encuesta DataEncuesta { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Idiomas { get; set; }
        public List<SelectListItem> Paises { get; set; }
        public ViewFindPollAssign Modelo { get; set; }
        public List<ViewUser> Usuarios { get; set; }
        public string SelectUser { get; set; }
    }
}