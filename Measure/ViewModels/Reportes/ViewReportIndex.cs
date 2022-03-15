using Measure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Reportes
{
    public class ViewReportIndex
    {
        public List<Models.Reporte> Lista { get; set; }

        public List<SelectListItem> Clientes { get; set; }

        public List<SelectListItem> TipoComponente { get; set; }

        public ViewReport Modelo { get; set; }        

        public DbAcciones Acciones { get; set; }

        public Guid ClienteId { get; set; }
    }
}