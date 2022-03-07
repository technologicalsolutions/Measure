using Measure.Models;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswerQuestionBase
    {
        public Models.Pregunta Question { get; set; }
        public ControlAbierto ControlA { get; set; }
        public ControlCerrado ControlC { get; set; }
        public ControlMatriz ControlM { get; set; }
    }
}