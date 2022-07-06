using System;
using System.Collections.Generic;

namespace Measure.ViewModels.Analitic
{
    public class GraphicBase
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public List<string> Categories { get; set; }
        public List<DataSeries> Series { get; set; }
        public List<string> Colors { get; set; }
    }
}