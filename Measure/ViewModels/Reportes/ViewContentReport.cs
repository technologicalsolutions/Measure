using Measure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Reportes
{
    public class ViewContentReport
    {
        public DbAcciones Acciones { get; set; }

        public Guid Id { get; set; }

        public Guid ReporteId { get; set; }

        public int Idioma { get; set; }

        public int TipoContenido { get; set; }

        [AllowHtml]
        public string Html { get; set; }

        public int Orden { get; set; }

        public bool Estado { get; set; }

        public List<SelectListItem> Idiomas { get; set; }
    }
}