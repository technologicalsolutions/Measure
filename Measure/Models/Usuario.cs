namespace Measure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Usuario")]
    public partial class Usuario
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [StringLength(150)]
        public string Clave { get; set; }

        public int RolId { get; set; }

        public Guid? ClienteId { get; set; }

        public Guid? AliadoId { get; set; }

        public byte[] Imagen { get; set; }

        public string Color { get; set; }

        public int Idioma { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombres { get; set; }

        [StringLength(200)]
        public string Apellidos { get; set; }

        [StringLength(500)]
        public string Telefono { get; set; }

        [StringLength(500)]
        public string Titulo { get; set; }

        [StringLength(500)]
        public string CorreoTrabajo { get; set; }

        [StringLength(500)]
        public string Empresa { get; set; }

        public int? CEmpleadosId { get; set; }

        public int? IndustriaId { get; set; }

        [StringLength(500)]
        public string Subsidiario { get; set; }

        public int? PaisId { get; set; }

        [StringLength(500)]
        public string DireccionTrabajo { get; set; }

        public bool Estado { get; set; }

        [StringLength(200)]
        public string Provincia { get; set; }
    }
}
