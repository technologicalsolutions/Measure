using Measure.Models;
using System;
using System.Linq;

namespace Measure.ViewModels.Dashboard
{
    public class ViewDashboardBasicDescription
    {
        public int Idioma { get; set; }
        public string Correo { get; set; }
        public string Nombres { get; set; }        
        public string Apellidos { get; set; }
        public int? IdPais { get; set; }
        public string Pais { get { return NombrePais(); } }
        public DateTime? FAsignado { get; set; }
        public DateTime? FCompletado { get; set; }
        public string Estado { get { return (FCompletado == null ? "Sin Responder" : "Respondida");  } }
        public double Dias { get { return TotalDias(); } }

        private string NombrePais()
        {
            MaestrasDetalle Pais = new MaestrasDetalle();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Pais = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.FirstOrDefault(d => d.Valor.Equals(IdPais.ToString()));
            }
            return Idioma == (int)Enums.Idiomas.es_ES ? Pais.es_ES : Idioma == (int)Enums.Idiomas.en_US ? Pais.en_US : Pais.pt_BR;
        }

        private int TotalDias()
        {
            if (FCompletado != null)
            {
                TimeSpan Between = (DateTime)FAsignado - (DateTime)FAsignado;
                return Convert.ToInt32(Between.TotalDays);
            }
            else
            {
                return 0;
            }
        }
    }
}