using Measure.Enums;
using Measure.Models;
using Measure.ViewModels.Contenidos;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class ContenidoController : Controller
    {
        [Route("Contenido")]
        public ActionResult Index(Guid? Cliente)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewContenidoEncuesta modelo = new ViewContenidoEncuesta
            {
                Accion = DbAcciones.Crea,
                Clientes = new List<SelectListItem>(),
                Lista = new List<Contenido>(),
                Modelo = new ViewContent
                {
                    ClienteId = Cliente
                },
                TipoComponente = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Inicio", Value = ((int)Enums.TipoComponente.Inicio).ToString() },
                    new SelectListItem { Text = "Fin", Value = ((int)Enums.TipoComponente.Fin).ToString() },
                    new SelectListItem { Text = "Grabar", Value =((int)Enums.TipoComponente.Grabar).ToString() },
                }
            };

            List<Contenido> lista = new List<Contenido>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                lista = (from A in db.Contenido
                         where ((Cliente == null || Cliente == Guid.Empty) || A.ClienteId == Cliente)
                         select A).ToList();               

                if (Cliente == null || Cliente == Guid.Empty)
                {
                    modelo.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                }
            }

            foreach (Contenido A in lista)
            {
                modelo.Lista.Add(new Contenido
                {
                    ClienteId = A.ClienteId,
                    en_US = A.en_US,
                    Estado = A.Estado,
                    es_Es = A.es_Es,
                    Id = A.Id,
                    Idioma = A.Idioma,
                    Posicion = A.Posicion,
                    pt_BR = A.pt_BR,
                    TipoContenido = A.TipoContenido,
                });
            }

            return View(modelo);
        }

        [HttpPost]
        public ActionResult CreateOrEdit(ViewContent Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            if (Data.Accion == (int)DbAcciones.Crea)
            {
                ViewBag.Title = "Crear Contenido";
            }
            else
            {
                ViewBag.Title = "Editar Contenido";
                Guid Id = Data.Id;
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Data = ResultViewContent(db.Contenido.Find(Id));
                }
            }
            return View(Data);
        }

        [HttpPost]
        public ActionResult CreateOrEditSave(ViewContent Data)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }
            else
            {
                if (Data.Accion == (int)DbAcciones.Crea)
                {
                    Contenido Create = ResultContenido(Data);
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        db.Contenido.Add(Create);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Contenido New = ResultContenido(Data);
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Contenido Update = UpdateContenido(db.Contenido.Find(Data.Id), New);
                        db.Entry(Update).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                Guid Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? Guid.Empty : Login.RolId == (int)UserRol.Cliente ? Login.Id : (Guid)Login.ClienteId;
                return RedirectToRoute("Contenido", Cliente);
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }
            else
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Contenido Eliminar = db.Contenido.Find(id);
                    db.Contenido.Remove(Eliminar);
                    db.SaveChanges();
                }

                Guid Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? Guid.Empty : Login.RolId == (int)UserRol.Cliente ? Login.Id : (Guid)Login.ClienteId;
                return RedirectToRoute("Contenido", Cliente);
            }
        }

        private Contenido ResultContenido(ViewContent Data)
        {
            Contenido Result = new Contenido
            {
                Html = Data.Html,
                ClienteId = (Guid)Data.ClienteId,
                Estado = Data.Estado,
                Id = Data.Id == null || Data.Id == Guid.Empty ? Guid.NewGuid() : Data.Id,
                Idioma = Data.Idioma,
                Posicion = Data.Posicion,
                TipoContenido = Data.TipoContenido,
                en_US = string.IsNullOrEmpty(Data.en_US) ? string.Empty : Data.en_US,
                es_Es = string.IsNullOrEmpty(Data.es_Es) ? string.Empty : Data.es_Es,
                pt_BR = string.IsNullOrEmpty(Data.pt_BR) ? string.Empty : Data.pt_BR,
            };
            return Result;
        }

        private ViewContent ResultViewContent(Contenido Data)
        {
            ViewContent Result = new ViewContent
            {
                Html = Data.Html,
                ClienteId = (Guid)Data.ClienteId,
                Estado = Data.Estado,
                Id = Data.Id == null || Data.Id == Guid.Empty ? Guid.NewGuid() : Data.Id,
                Idioma = Data.Idioma,
                Posicion = Data.Posicion,
                TipoContenido = Data.TipoContenido,
                en_US = string.IsNullOrEmpty(Data.en_US) ? string.Empty : Data.en_US,
                es_Es = string.IsNullOrEmpty(Data.es_Es) ? string.Empty : Data.es_Es,
                pt_BR = string.IsNullOrEmpty(Data.pt_BR) ? string.Empty : Data.pt_BR,
            };
            return Result;
        }

        private Contenido UpdateContenido(Contenido Old, Contenido New)
        {
            Old.ClienteId = Old.ClienteId != New.ClienteId ? New.ClienteId : Old.ClienteId;
            Old.en_US = Old.en_US != New.en_US ? New.en_US : Old.en_US;
            Old.Estado = Old.Estado != New.Estado ? New.Estado : Old.Estado;
            Old.es_Es = Old.es_Es != New.es_Es ? New.es_Es : Old.es_Es;
            Old.Html = Old.Html != New.Html ? New.Html : Old.Html;
            Old.Posicion = Old.Posicion != New.Posicion ? New.Posicion : Old.Posicion;
            Old.pt_BR = Old.pt_BR != New.pt_BR ? New.pt_BR : Old.pt_BR;
            Old.TipoContenido = Old.TipoContenido != New.TipoContenido ? New.TipoContenido : Old.TipoContenido;

            return Old;
        }
    }
}