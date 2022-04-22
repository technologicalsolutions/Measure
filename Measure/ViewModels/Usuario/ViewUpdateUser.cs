using Measure.Enums;
using Measure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Measure.ViewModels.Usuario
{
    public class ViewUpdateUser
    {
        public List<ViewUserProperties> DataPage { get; set; }

        public Models.Grupo Group { get; set; }
                
        public Guid Id { get; set; }

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
        [System.ComponentModel.DataAnnotations.Compare("CorreoTrabajo")]
        public string ConfirmarCorreoTrabajo { get; set; }

        [StringLength(500)]
        public string Empresa { get; set; }

        public int CEmpleadosId { get; set; }

        public List<SelectListItem> CEmpleados { get { return CantidadEmpleados(); } }

        public int IndustriaId { get; set; }

        public List<SelectListItem> Industrias { get { return ListaIndustrias(); } }

        [StringLength(500)]
        public string Subsidiario { get; set; }

        public int? PaisId { get; set; }

        public List<SelectListItem> Paises { get { return ListaPaises(); } }

        [StringLength(500)]
        public string DireccionTrabajo { get; set; }

        public bool Estado { get; set; }

        [StringLength(200)]
        public string Provincia { get; set; }

        private List<SelectListItem> CantidadEmpleados()
        {
            List<MaestrasDetalle> Empleados = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Empleados = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Empleados")).MaestrasDetalle.Where(d => d.Estado).ToList();
            }
            return Empleados.Select(s => new SelectListItem
            {
                Text = Idioma == (int)Idiomas.es_ES ? s.es_ES : Idioma == (int)Idiomas.en_US ? s.en_US : s.pt_BR,
                Value = s.Id.ToString()
            }).OrderBy(o => o.Text).ToList();
        }

        private List<SelectListItem> ListaIndustrias()
        {
            List<MaestrasDetalle> Industrias = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Industrias = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Industrias")).MaestrasDetalle.Where(d => d.Estado).ToList();
            }
            return Industrias.Select(s => new SelectListItem
            {
                Text = Idioma == (int)Idiomas.es_ES ? s.es_ES : Idioma == (int)Idiomas.en_US ? s.en_US : s.pt_BR,
                Value = s.Id.ToString()
            }).OrderBy(o => o.Text).ToList();
        }

        private List<SelectListItem> ListaPaises()
        {
            List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.Where(d => d.Estado).ToList();
            }
            return Paises.Select(s => new SelectListItem
            {
                Selected = (PaisId.ToString() == s.Valor),
                Text = Idioma == (int)Idiomas.es_ES ? s.es_ES : Idioma == (int)Idiomas.en_US ? s.en_US : s.pt_BR,
                Value = s.Valor.ToString()
            }).OrderBy(o => o.Text).ToList();
        }
    }
}