using System.Collections.Generic;

namespace Measure.ViewModels.Analitic
{
    public class DataSeries
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<decimal> data { get; set; }
    }
}