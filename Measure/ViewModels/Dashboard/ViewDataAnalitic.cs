using System;
using System.Collections.Generic;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDataAnalitic
    {
        public List<ViewDataAnaliticDetail> Encuestados { get; set; }
        public int TipoAnalisis { get; set; }
        public Guid EncuestaId { get; set; }
    }
}