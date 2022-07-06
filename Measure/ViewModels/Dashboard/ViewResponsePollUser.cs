using System;

namespace Measure.ViewModels.Dashboard
{
    public class ViewResponsePollUser
    {
        public Guid Id { get; set; }
        public string Correo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int? PaisId { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string Sucursal { get; set; }
        public string Gerencia { get; set; }
        public string Rol { get; set; }
    }
}