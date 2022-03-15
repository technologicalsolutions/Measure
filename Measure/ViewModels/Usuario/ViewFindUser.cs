using System;

namespace Measure.ViewModels.Usuario
{
    public class ViewFindUser
    {
        public Guid EncuestaId { get; set; }

        public Guid ClienteId { get; set; }

        public int? PaisId { get; set; }        
        
        public int? Idioma { get; set; }

        public string Correo { get; set; }

        public string Nombre { get; set; }
    }
}