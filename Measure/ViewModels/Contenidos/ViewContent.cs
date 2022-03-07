using Measure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Measure.ViewModels.Contenidos
{
    public class ViewContent
    {
        public DbAcciones Accion { get; set; }

        public Guid Id { get; set; }

        public Guid? ClienteId { get; set; }

        [StringLength(250)]
        public string es_Es { get; set; }

        [StringLength(250)]
        public string en_US { get; set; }

        [StringLength(250)]
        public string pt_BR { get; set; }

        public int? Idioma { get; set; }

        public bool Posicion { get; set; }

        [AllowHtml]
        public string Html { get; set; }

        public int TipoContenido { get; set; }

        public bool Estado { get; set; }
    }
}