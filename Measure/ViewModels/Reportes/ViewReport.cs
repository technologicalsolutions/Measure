using System;
using System.ComponentModel.DataAnnotations;

namespace Measure.ViewModels.Reportes
{
    public class ViewReport
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        [StringLength(250)]
        public string es_ES { get; set; }

        [StringLength(250)]
        public string en_US { get; set; }

        [StringLength(250)]
        public string pt_BR { get; set; }

        public bool Estado { get; set; }
    }
}