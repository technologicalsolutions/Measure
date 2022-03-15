using Measure.Enums;
using Measure.ViewModels.Grupo;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Encuesta
{
    public class ViewEncuesta
    {
        public List<SelectListItem> Idiomas { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> TipoReporteGeneral { get; set; }
        public List<ViewPoll> Lista { get; set; }
        public ViewPoll Modelo { get; set; }
        public Models.Usuario Cliente { get; set; }
        public Guid? ClienteId { get; set; }
        public int RolId { get; set; }
        public DbAcciones Accion { get; set; }
    }
}