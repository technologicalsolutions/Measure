using Measure.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Contenidos
{
    public class ViewContenidoEncuesta
    {
        public List<Models.Contenido> Lista { get; set; }

        public List<SelectListItem> Clientes { get; set; }

        public List<SelectListItem> TipoComponente { get; set; }

        public ViewContent Modelo { get; set; }

        public int Idioma { get; set; }

        public DbAcciones Accion { get; set; }
    }
}