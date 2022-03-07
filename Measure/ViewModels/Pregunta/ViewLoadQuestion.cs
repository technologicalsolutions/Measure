using Measure.Models;
using Measure.ViewModels.Respuesta;

namespace Measure.ViewModels.Pregunta
{
    public class ViewLoadQuestion
    {
        public Models.Pregunta Question { get; set; }

        public ControlAbierto ControlesAbiertos { get; set; }

        public ControlBooleano ControlesBooleanos { get; set; }

        public ControlCerrado ControlesCerrados { get; set; }

        public ViewAnswerControlMatriz ControlesMatrices { get; set; }

        public Models.Respuesta Answer { get; set; }
    }
}