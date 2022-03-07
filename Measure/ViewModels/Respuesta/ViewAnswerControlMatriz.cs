using Measure.Models;
using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswerControlMatriz
    {
        public Models.ControlMatriz ControlM { get; set; }

        public List<ControlMatrizColumna> ControlMColunnas { get; set; }

        public List<ControlMatrizFila> ControlMFilas { get; set; }
    }
}