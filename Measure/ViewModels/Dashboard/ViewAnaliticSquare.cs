using Measure.ViewModels.Analitic;
using System.Collections.Generic;

namespace Measure.ViewModels.Dashboard
{
    public class ViewAnaliticSquare
    {
        public string TituloGeneral { get; set; }
        public List<GraphicBase> CuadroUno { get; set; }
        public GraphicTwo CuadroDos { get; set; }

    }    
}