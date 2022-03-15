using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Measure.Models
{
    public partial class ModeloEncuesta : DbContext
    {
        public ModeloEncuesta()
            : base("name=ModeloEncuesta")
        {
        }

        public virtual DbSet<Contenido> Contenido { get; set; }
        public virtual DbSet<ContenidoPorEncuesta> ContenidoPorEncuesta { get; set; }
        public virtual DbSet<ContenidoReporte> ContenidoReporte { get; set; }
        public virtual DbSet<ControlAbierto> ControlAbierto { get; set; }
        public virtual DbSet<ControlBooleano> ControlBooleano { get; set; }
        public virtual DbSet<ControlCerrado> ControlCerrado { get; set; }
        public virtual DbSet<ControlMatriz> ControlMatriz { get; set; }
        public virtual DbSet<ControlMatrizColumna> ControlMatrizColumna { get; set; }
        public virtual DbSet<ControlMatrizFila> ControlMatrizFila { get; set; }
        public virtual DbSet<Encuesta> Encuesta { get; set; }
        public virtual DbSet<Grupo> Grupo { get; set; }
        public virtual DbSet<Maestras> Maestras { get; set; }
        public virtual DbSet<MaestrasDetalle> MaestrasDetalle { get; set; }
        public virtual DbSet<Pregunta> Pregunta { get; set; }
        public virtual DbSet<PreguntasPorGrupo> PreguntasPorGrupo { get; set; }
        public virtual DbSet<Reporte> Reporte { get; set; }
        public virtual DbSet<Respuesta> Respuesta { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<UsuarioLabel> UsuarioLabel { get; set; }
        public virtual DbSet<UsuariosPorEncuenta> UsuariosPorEncuenta { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ControlMatriz>()
                .HasMany(e => e.ControlMatrizColumna)
                .WithRequired(e => e.ControlMatriz)
                .HasForeignKey(e => e.MatrizId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ControlMatriz>()
                .HasMany(e => e.ControlMatrizFila)
                .WithRequired(e => e.ControlMatriz)
                .HasForeignKey(e => e.MatrizId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Maestras>()
                .HasMany(e => e.MaestrasDetalle)
                .WithRequired(e => e.Maestras)
                .HasForeignKey(e => e.MaestraId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reporte>()
               .HasMany(e => e.ContenidosReporte)
               .WithRequired(e => e.Reporte)
               .HasForeignKey(e => e.ReporteId)
               .WillCascadeOnDelete(false);
        }
    }
}
