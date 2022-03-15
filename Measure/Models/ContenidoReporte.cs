namespace Measure.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ContenidoReporte")]
    public class ContenidoReporte
    {        
        public Guid Id { get; set; }

        public Guid ReporteId { get; set; }

        public int Idioma { get; set; }

        public int TipoContenido { get; set; }

        public string Html { get; set; }

        public int Orden { get; set; }

        public bool Estado { get; set; }

        public string Nombre { get; set; }

        public virtual Reporte Reporte { get; set; }
    }
}