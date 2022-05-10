using Measure.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Analitic
{
    public class ViewIndex
    {
        public DbAcciones Accion { get; set; }
        public List<SelectListItem> Encuestas { get; set; }
        public List<SelectListItem> Grupos { get; set; }
        public List<ViewAnalitic> Parametros { get; set; }
        public ViewAnalitic Modelo { get; set; }
    }
}