using Measure.Models;
using Measure.ViewModels.Grupo;
using System.Collections.Generic;

namespace Measure.ViewModels.ContenidoPorEncuesta
{
    public class ViewContentList
    {
        public Models.Encuesta Padre { get; set; }
        public Models.ContenidoPorEncuesta Modelo { get; set; }
        public List<Contenido> Contenidos { get; set; }
        public List<ViewContentGroup> Grupos { get; set; }        
    }
}