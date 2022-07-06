using Measure.ViewModels.Analitic;
using System;
using System.Collections.Generic;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDataAnalitic
    {
        public List<ViewDataAnaliticDetail> Encuestados { get; set; }        
        public Guid EncuestaId { get; set; }
        public ViewAnaliticFilter Filtros { get; set; }
    }
}