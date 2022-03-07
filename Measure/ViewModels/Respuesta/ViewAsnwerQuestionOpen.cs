using System;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAsnwerQuestionOpen : ViewAnswerQuestionBase
    {
        public Tuple<Guid, string, string, string> Contenido { get; set; }
    }
}