using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Usuario
{
    public class ViewLogin : ViewUser
    {
        public Enums.Idiomas Lenguaje { get; set; }
        public List<SelectListItem> Lenguajes { get; set; }

    }
}