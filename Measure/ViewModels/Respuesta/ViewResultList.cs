using System;
using System.ComponentModel.DataAnnotations;

namespace Measure.ViewModels.Respuesta
{
    public class ViewResultList
    {
        public Guid UsuarioId { get; set; }

        [Display(Name = "Nombre Cliente")]
        public string NombreCliente { get; set; }

        public string Correo { get; set; }

        public string Usuario { get; set; }

        [Display(Name = "Fecha Respuesta")]
        public DateTime FechaRespuesta { get; set; }
    }
}