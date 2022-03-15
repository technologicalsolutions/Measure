using Measure.Models;
using Measure.ViewModels.Reportes;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class ReporteController : Controller
    {
        [Route("Reportes")]
        public ActionResult Index(Guid? Cliente)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewReportIndex Model = new ViewReportIndex
            {
                Acciones = Enums.DbAcciones.Crea,
                ClienteId = Cliente == null ? Guid.Empty : (Guid)Cliente,
                Lista = new List<Reporte>()
            };

            List<Reporte> lista = new List<Reporte>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (Cliente == null || Cliente == Guid.Empty)
                {
                    Model.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                }
                lista = (from A in db.Reporte
                         where ((Cliente == null || Cliente == Guid.Empty) || A.ClienteId == Cliente)
                         select A).ToList();
            }

            foreach (Reporte A in lista)
            {
                Model.Lista.Add(new Reporte
                {
                    ClienteId = A.ClienteId,
                    en_US = A.en_US,
                    Estado = A.Estado,
                    es_ES = A.es_ES,
                    Id = A.Id,
                    pt_BR = A.pt_BR,
                });
            }

            return View(Model);
        }

        [Route("ReporteAcciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Acciones(ViewReportIndex Data)
        {
            if (Data.Acciones == Enums.DbAcciones.Crea)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Reporte AddItem = ConvertViewReportToReport(Data.Modelo);
                    db.Reporte.Add(AddItem);
                    db.SaveChanges();
                }
            }
            else if (Data.Acciones == Enums.DbAcciones.Actualiza)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Reporte Old = db.Reporte.Find(Data.Modelo.Id);
                    Reporte Update = UpdateReport(Old, Data.Modelo);
                    db.Entry(Update).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToRoute("Reportes", new { Id = Data.ClienteId });
        }

        private Reporte ConvertViewReportToReport(ViewReport Data)
        {
            return new Reporte
            {
                ClienteId = Data.ClienteId,
                en_US = Data.en_US,
                Estado = Data.Estado,
                es_ES = Data.es_ES,
                Id = Data.Id == Guid.Empty || Data.Id == null ? Guid.NewGuid() : Data.Id,
                pt_BR = Data.pt_BR,
            };
        }

        private Reporte UpdateReport(Reporte Old, ViewReport New)
        {
            Old.en_US = Old.en_US != New.en_US ? Old.en_US : New.en_US;
            Old.Estado = Old.Estado != New.Estado ? Old.Estado : New.Estado;
            Old.es_ES = Old.es_ES != New.es_ES ? Old.es_ES : New.es_ES;
            Old.pt_BR = Old.pt_BR != New.pt_BR ? Old.pt_BR : New.pt_BR;

            return Old;
        }

        [HttpPost]
        public PartialViewResult Detail(ViewReportIndex Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Reporte AddItem = db.Reporte.Find(Data.Modelo.Id);
                return PartialView("_Details", AddItem);
            }
        }

        [HttpPost, Route("EliminarReporte")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid Id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Reporte DeleteItem = db.Reporte.Find(Id);
                db.Reporte.Remove(DeleteItem);
                db.SaveChanges();
            }

            return RedirectToRoute("Reportes", new { Id = Login.ClienteId });
        }

        [Route("ContenidosReporte")]
        public ActionResult Contenido(Guid Id)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewContentReportIndex Model = new ViewContentReportIndex
            {
                Acciones = Enums.DbAcciones.Crea,
                Lista = new List<ContenidoReporte>(),
                ReporteId = Id,
                TipoContenido = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Inicio", Value = ((int)Enums.TipoContenido.Inicio).ToString() },
                    new SelectListItem { Text = "Fin", Value = ((int)Enums.TipoContenido.Fin).ToString() },
                }
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Model.Padre = db.Reporte.Find(Id);
                Model.Lista.AddRange(db.ContenidoReporte.Where(c => c.ReporteId == Id).ToList());
            }
            return View(Model);
        }

        [Route("ContenidoReporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ViewContentReport Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            Enums.Idiomas BasePlataforma = (Enums.Idiomas)HttpContext.Session["BaseIdioma"];
            ViewContentReport Model = new ViewContentReport
            {
                Acciones = Data.Acciones,
                ReporteId = Data.ReporteId,                
            };
            
            if (Data.Acciones == Enums.DbAcciones.Actualiza)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Model = ConvertContenidoReporteToViewContentReport(db.ContenidoReporte.Find(Data.Id));
                    Model.Acciones = Data.Acciones;
                }
            }

            List<MaestrasDetalle> Lenguaje = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Lenguaje = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.ToList();
            }
            switch (BasePlataforma)
            {
                case Enums.Idiomas.es_ES:
                    Model.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.es_ES, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
                case Enums.Idiomas.en_US:
                    Model.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.en_US, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
                case Enums.Idiomas.pt_BR:
                    Model.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.pt_BR, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
            }

            ViewBag.Title = Data.Acciones == Enums.DbAcciones.Crea ? "Crear Contenido" : "Editar Contenido";
            return View(Model);
        }

        private ViewContentReport ConvertContenidoReporteToViewContentReport(ContenidoReporte Data)
        {
            return new ViewContentReport
            {
                Estado = Data.Estado,
                Html = Data.Html,
                Id = Data.Id,
                Idioma = Data.Idioma,
                Orden = Data.Orden,
                ReporteId = Data.ReporteId,
                TipoContenido = Data.TipoContenido,
            };
        }

        private ContenidoReporte ConvertViewContentReportToContenidoReporte(ViewContentReport Data)
        {
            return new ContenidoReporte
            {
                Estado = Data.Estado,
                Html = Data.Html,
                Id = Data.Id == Guid.Empty ? Guid.NewGuid() : Data.Id,
                Idioma = Data.Idioma,
                Orden = Data.Orden,
                ReporteId = Data.ReporteId,
                TipoContenido = Data.TipoContenido,
            };
        }

        [Route("AccionesContenido")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionContent(ViewContentReport Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            Guid idReporte = Data.ReporteId;
            if (Data.Acciones == Enums.DbAcciones.Crea)
            {
                ContenidoReporte AddItem = ConvertViewContentReportToContenidoReporte(Data);
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    db.ContenidoReporte.Add(AddItem);
                    db.SaveChanges();
                }
            }
            else
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ContenidoReporte Old = db.ContenidoReporte.Find(Data.Id);
                    ContenidoReporte UpdateItem = UpdateContenido(Old, Data);

                    db.Entry(UpdateItem).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            return RedirectToRoute("ContenidosReporte", new { Id = idReporte });
        }

        private ContenidoReporte UpdateContenido(ContenidoReporte Old, ViewContentReport New)
        {
            Old.Estado = Old.Estado != New.Estado ? New.Estado : Old.Estado;
            Old.Html = Old.Html != New.Html ? New.Html : Old.Html;
            Old.Idioma = Old.Idioma != New.Idioma ? New.Idioma : Old.Idioma;
            Old.Orden = Old.Orden != New.Orden ? New.Orden : Old.Orden;
            Old.TipoContenido = Old.TipoContenido != New.TipoContenido ? New.TipoContenido : Old.TipoContenido;

            return Old;
        }

        [HttpPost, Route("EliminarContenido")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteContent(Guid Id)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            Guid idReporte = Guid.Empty;
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ContenidoReporte DeleteItem = db.ContenidoReporte.Find(Id);
                idReporte = DeleteItem.ReporteId;
                db.ContenidoReporte.Remove(DeleteItem);
                db.SaveChanges();
            }

            return RedirectToRoute("ContenidosReporte", new { Id = idReporte });
        }
    }
}