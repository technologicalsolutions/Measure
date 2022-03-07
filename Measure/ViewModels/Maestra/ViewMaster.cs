using Measure.Enums;
using Measure.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Maestra
{
    public class ViewMaster
    {
        public DbAcciones Acciones { get; set; }

        public List<SelectListItem> Clientes { get; set; }

        public List<Maestras> Lista { get; set; }

        public Maestras Modelo { get; set; }
    }
}