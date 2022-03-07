using System.Collections.Generic;

namespace Measure.ViewModels.Respuesta
{
    public class ViewAnswerSurvey
    {
        public Models.Usuario Cliente { get; set; }        
        
        public List<ViewContentListAnswer> Lista { get; set; }        

        public Models.UsuariosPorEncuenta Asignacion { get; set; }
    }
}