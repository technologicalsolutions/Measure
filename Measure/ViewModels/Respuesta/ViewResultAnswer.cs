using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Respuesta
{
    public class ViewResultAnswer
    {
        public List<SelectListItem> Clientes { get; set; }
        public List<ViewResultList> Lista { get; set; }
    }
}