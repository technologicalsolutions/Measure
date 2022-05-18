using System.Collections.Generic;

namespace Measure.ViewModels.Analitic
{
    public class GraphicBase
    {
        public List<string> Categories { get; set; }
        public List<DataSeries> Series { get; set; }
        public List<string> Colors { get; set; }
    }
}