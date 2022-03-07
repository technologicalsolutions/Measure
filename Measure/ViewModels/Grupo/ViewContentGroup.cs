using System;

namespace Measure.ViewModels.Grupo
{
    public class ViewContentGroup
    {
        public Guid Id { get; set; }
        
        public string es_Es { get; set; }
        
        public string en_US { get; set; }
        
        public string pt_BR { get; set; }

        public int CantidadPreguntas { get; set; }
    }
}