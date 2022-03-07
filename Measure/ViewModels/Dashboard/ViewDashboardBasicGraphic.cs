using System.Collections.Generic;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDashboardBasicGraphic
    {
        public ViewDashboardBasic DatosBasicos { get; set; }
        public List<ViewDashboardBasic> RespuestasXGrupo { get; set; }
        public List<ViewDashboardDistCount> DistPais { get; set; }
    }
}