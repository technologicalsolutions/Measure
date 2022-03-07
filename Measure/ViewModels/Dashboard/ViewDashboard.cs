using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDashboard
    {
        public string ClienteId { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Encuestas { get; set; }
        public List<ViewDashboardClientForPoll> GraphGeneral { get; set; }
    }
}