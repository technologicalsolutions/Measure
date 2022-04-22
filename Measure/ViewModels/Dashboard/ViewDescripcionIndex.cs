using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDescripcionIndex
    {
        public Models.Usuario User { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Encuestas { get; set; }
        public int PartialGraph { get; set; }
    }
}