using Measure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Reportes
{
    public class ViewContentReportIndex
    {
        public List<Models.ContenidoReporte> Lista { get; set; }

        public List<SelectListItem> TipoContenido { get; set; }

        public Models.Reporte Padre { get; set; }

        public DbAcciones Acciones { get; set; }

        public Guid ReporteId { get; set; }
    }
}