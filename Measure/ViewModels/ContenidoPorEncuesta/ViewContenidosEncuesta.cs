using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.ContenidoPorEncuesta
{
    public class ViewContenidosEncuesta
    {
        public Models.Encuesta Padre { get; set; }
        public List<ViewSurveyContent> Lista { get; set; }
        public List<SelectListItem> TiposComponente { get; set; }
        
    }
}