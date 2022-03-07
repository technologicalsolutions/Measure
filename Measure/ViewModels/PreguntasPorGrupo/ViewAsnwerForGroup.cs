using System.Collections.Generic;

namespace Measure.ViewModels.PreguntasPorGrupo
{
    public class ViewAsnwerForGroup
    {
        public Models.Grupo Group { get; set; }
        public List<ViewAnswerGroup> Questions { get; set; }

        public Models.PreguntasPorGrupo Modelo { get; set; }

    }
}