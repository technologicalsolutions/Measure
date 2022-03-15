using System.ComponentModel.DataAnnotations;

namespace Measure.Models
{
    public class UsuarioLabel
    {
        [Key]
        public string Campo { get; set; }        
        public string es_ES { get; set; }
        public string en_US { get; set; }
        public string pt_BR { get; set; }
        public int Orden { get; set; }
        public bool Confirmacion { get; set; }
        public bool Comparar { get; set; }
    }
}