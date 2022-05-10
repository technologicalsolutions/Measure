using Measure.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Usuario
{
    public class ViewUsuario
    {
        public List<ViewUser> Lista { get; set; }
        public List<SelectListItem> Paises { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Aliados { get; set; }
        public List<SelectListItem> Idiomas { get; set; }
        public ViewUser Modelo { get; set; }
        public DbAcciones Accion { get; set; }
        public ViewLogin FindUser { get; set; }
    }
}