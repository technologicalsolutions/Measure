using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswerGroupPoll
    {
        public Models.Grupo Group { get; set; }
        public virtual List<ViewAsnwerQuestionOpen> RespuestasAbierta { get; set; }
        public virtual List<ViewAsnwerQuestionMatriz> RespuestasMatriz { get; set; }
    }
}