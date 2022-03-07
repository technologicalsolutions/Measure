using Measure.Enums;
using Measure.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Pregunta
{
    public class ViewQuestion : ViewPregunta
    {
        public DbAcciones Accion { get; set; }

        public List<SelectListItem> Maestras { get; set; }

        public Models.ControlAbierto ControlA { get; set; }

        public Models.ControlCerrado ControlC { get; set; }

        public ControlBooleano ControlB { get; set; }

        public Models.ControlMatriz ControlM { get; set; }

        public List<ControlMatrizColumna> ControlMColunnas { get; set; }

        public List<ControlMatrizFila> ControlMFilas { get; set; }
    }
}