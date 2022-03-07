using Measure.ViewModels.Pregunta;
using System.Collections.Generic;

namespace Measure.ViewModels.Grupo
{
    public class ViewLoadGroupPoll
    {        

        public Models.Grupo Group { get; set; }

        public virtual List<ViewLoadQuestion> Preguntas { get; set; }
    }
}