using System;
using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewResultMatrizRange
    {
        public RadialBar Radial { get; set; }
        public List<Tuple<int, string>> Barra { get; set; }
    }
    public class RadialBar
    {
        public int Serie { get; set; }
        public string Label { get; set; }

    }
}