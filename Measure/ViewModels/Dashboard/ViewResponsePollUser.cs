using System;

namespace Measure.ViewModels.Dashboard
{
    public class ViewResponsePollUser
    {
        public Guid Id { get; set; }
        public string Correo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaRespuesta { get; set; }
    }
}