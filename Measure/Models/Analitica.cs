using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Measure.Models
{
    [Table("Analitica")]
    public class Analitica
    {
        [Key]
        public Int64 Id { get; set; }
        public int TipoParametroId { get; set; }
        public Guid? GrupoId { get; set; }
        public string Variable1 { get; set; }
        public string Variable2 { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
        public string Alcance { get; set; }
        public string CalificacionMatematica { get; set; }
        public bool Dimensiones { get; set; }
        public string CalificacionConceptual { get; set; }
        public string MadurezMatematica { get; set; }
        public string MadurezConceptual { get; set; }
        public string DescripcionRiesgos { get; set; }
        public string DescripcionActividadesEm { get; set; }
        public string DescripcionActividadesMa { get; set; }
        public string TituloGrafico { get; set; }
        public string SubTituloGrafico { get; set; }
        public bool Estado { get; set; }

        public virtual Grupo Grupo { get; set; }
    }
}