using Measure.Enums;
using Measure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.ViewModels.Dashboard
{
    public class ViewResponsePartial
    {
        public int Idioma { get; set; }

        public List<ViewResponsePollUser> Encuestados { get; set; }

        public List<SelectListItem> Paises { get { return ListaPaises(); } }

        private List<SelectListItem> ListaPaises()
        {
            List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.Where(d => d.Estado).ToList();
            }
            return Paises.Select(s => new SelectListItem
            {                
                Text = Idioma == (int)Idiomas.es_ES ? s.es_ES : Idioma == (int)Idiomas.en_US ? s.en_US : s.pt_BR,
                Value = s.Valor
            }).OrderBy(o => o.Text).ToList();
        }
    }
}