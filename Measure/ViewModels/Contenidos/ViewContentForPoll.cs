using Measure.Models;
using Measure.ViewModels.PreguntasPorGrupo;
using System.Collections.Generic;

namespace Measure.ViewModels.Contenidos
{
    public class ViewContentForPoll
    {
        public List<Contenido> Contenidos { get; set; }
        public List<ViewAsnwerForGroup> Pregunas { get; set; }        
    }
}