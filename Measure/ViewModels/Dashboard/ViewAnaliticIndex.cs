using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Dashboard
{
    public class ViewAnaliticIndex
    {
        public Models.Usuario User { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Encuestas { get; set; }
        public List<SelectListItem> Aliados { get; set; }
        public List<SelectListItem> Paises { get; set; }
        public ViewResponsePartial PartialIndex { get; set; }
        
    }
}