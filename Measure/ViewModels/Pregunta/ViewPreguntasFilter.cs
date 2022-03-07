using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Measure.ViewModels.Pregunta
{
    public class ViewPreguntasFilter
    {
        public Guid? ClienteId { get; set; }
        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Idiomas { get; set; }        
        public List<SelectListItem> TipoControl { get; set; }
        public List<ViewPregunta> ListaPreguntas { get; set; }
    }
}