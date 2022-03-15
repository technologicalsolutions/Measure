using Measure.Models;
using Measure.ViewModels.Grupo;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class GruposController : Controller
    {
        [Route("Categorias")]
        public ActionResult Index(Guid? Cliente)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewGrupoEncuestas Modelo = new ViewGrupoEncuestas
            {
                Modelo = new ViewGroupPoll(),
                ClienteId = Cliente,                
            };

            if (Cliente == Guid.Empty)
            {
                Modelo.Lista = ListaDeEncuestas(null);
            }
            else
            {
                Modelo.Lista = ListaDeEncuestas(Cliente);
            }
            return View(Modelo);
        }

        private List<ViewGroupPoll> ListaDeEncuestas(Guid? Cliente)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<ViewGroupPoll> Lista = (from C in db.Grupo
                                             where (Cliente == null  || C.ClienteId == Cliente)
                                             select new ViewGroupPoll
                                             {
                                                 Columnas = C.Columnas,
                                                 Desc_en_US = C.Desc_en_US,
                                                 Desc_es_Es = C.Desc_es_Es,
                                                 Desc_pt_BR = C.Desc_pt_BR,
                                                 Estado = C.Estado,
                                                 Id = C.Id,
                                                 en_US = C.en_US,
                                                 es_Es = C.es_Es,
                                                 pt_BR = C.pt_BR,
                                             }).ToList();
                return Lista;
            }
        }

        [HttpPost]
        [Route("Categoria")]
        public ActionResult Grupo(ViewGrupoEncuestas Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Data.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
            }

            switch (Data.Accion)
            {
                case Enums.DbAcciones.Crea:
                    ViewBag.Title = "Crear";
                    Data.Modelo = new ViewGroupPoll();
                    break;
                case Enums.DbAcciones.Actualiza:
                    ViewBag.Title = "Editar";
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {                        
                        Data.Modelo = ResultViewGroupPoll(db.Grupo.Find(Data.Modelo.Id));
                    }
                    break;
            }
            return View("CreateOrEdit", Data);
        }

        private ViewGroupPoll ResultViewGroupPoll(Grupo Data)
        {
            ViewGroupPoll Result = new ViewGroupPoll
            {
                Columnas = Data.Columnas,
                Color = Data.Color,
                Desc_es_Es = Data.Desc_es_Es,
                Desc_en_US = Data.Desc_en_US,
                Desc_pt_BR = Data.Desc_pt_BR,                
                Estado = Data.Estado,
                Id = Data.Id,
                en_US = Data.en_US,
                es_Es = Data.es_Es,
                pt_BR = Data.pt_BR,                
                RespuestaAltaMin = Data.RespuestaAltaMin == null ? 0 : Data.RespuestaAltaMin,
                RespuestaBajaMax = Data.RespuestaBajaMax == null ? 0 : Data.RespuestaBajaMax,
                TipoReporte = Data.TipoReporte
            };

            return Result;
        }

        [HttpPost]
        [Route("CategoriasAcciones")]
        public ActionResult Acciones(ViewGrupoEncuestas Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Grupo Grupo = ResultGruposEncuesta(Data.Modelo);
                switch (Data.Accion)
                {
                    case Enums.DbAcciones.Crea:
                        db.Grupo.Add(Grupo);
                        break;
                    case Enums.DbAcciones.Actualiza:
                        Grupo _OldEncuesta = db.Grupo.Find(Grupo.Id);
                        Grupo OldEncuesta = CompareGroupPoll(_OldEncuesta, Grupo);
                        db.Entry(OldEncuesta).State = EntityState.Modified;
                        break;
                }
                db.SaveChanges();

                if (HttpContext.Session["login"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                    Guid? Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    return RedirectToRoute("Categorias", Cliente);
                }
            }
        }

        private Grupo ResultGruposEncuesta(ViewGroupPoll Data)
        {
            Grupo Result = new Grupo
            {
                Columnas = Data.Columnas,
                Color = Data.Color,
                ClienteId = Data.ClienteId,
                Desc_en_US = Data.Desc_en_US,
                Desc_es_Es = Data.Desc_es_Es,
                Desc_pt_BR = Data.Desc_pt_BR,
                Estado = Data.Estado,
                Id = Data.Id == null || Data.Id == Guid.Empty ? Guid.NewGuid() : (Guid)Data.Id,
                en_US = Data.en_US,
                es_Es = Data.es_Es,
                pt_BR = Data.pt_BR,
                RespuestaAltaMin = Data.RespuestaAltaMin == null ? 0 : Data.RespuestaAltaMin,
                RespuestaBajaMax = Data.RespuestaBajaMax == null ? 0 : Data.RespuestaBajaMax,
                TipoReporte = Data.TipoReporte,
            };
            return Result;
        }

        private Grupo CompareGroupPoll(Grupo OldGrupo, Grupo NewGrupo)
        {
            OldGrupo.Columnas = OldGrupo.Columnas != NewGrupo.Columnas ? NewGrupo.Columnas : OldGrupo.Columnas;
            OldGrupo.Color = OldGrupo.Color != NewGrupo.Color ? NewGrupo.Color : OldGrupo.Color;
            OldGrupo.Desc_en_US = OldGrupo.Desc_en_US != NewGrupo.Desc_en_US ? NewGrupo.Desc_en_US : OldGrupo.Desc_en_US;
            OldGrupo.Desc_es_Es = OldGrupo.Desc_es_Es != NewGrupo.Desc_es_Es ? NewGrupo.Desc_es_Es : OldGrupo.Desc_es_Es;
            OldGrupo.Desc_pt_BR = OldGrupo.Desc_pt_BR != NewGrupo.Desc_pt_BR ? NewGrupo.Desc_pt_BR : OldGrupo.Desc_pt_BR;
            OldGrupo.Estado = OldGrupo.Estado != NewGrupo.Estado ? NewGrupo.Estado : OldGrupo.Estado;
            OldGrupo.en_US = OldGrupo.en_US != NewGrupo.en_US ? NewGrupo.en_US : OldGrupo.en_US;
            OldGrupo.es_Es = OldGrupo.es_Es != NewGrupo.es_Es ? NewGrupo.en_US : OldGrupo.es_Es;
            OldGrupo.pt_BR = OldGrupo.pt_BR != NewGrupo.pt_BR ? NewGrupo.pt_BR : OldGrupo.pt_BR;
            OldGrupo.RespuestaAltaMin = OldGrupo.RespuestaAltaMin != NewGrupo.RespuestaAltaMin ? NewGrupo.RespuestaAltaMin == null ? 0 : NewGrupo.RespuestaAltaMin : OldGrupo.RespuestaAltaMin;
            OldGrupo.RespuestaBajaMax = OldGrupo.RespuestaBajaMax != NewGrupo.RespuestaBajaMax ? NewGrupo.RespuestaBajaMax == null ? 0 : NewGrupo.RespuestaBajaMax : OldGrupo.RespuestaBajaMax;
            OldGrupo.TipoReporte = OldGrupo.TipoReporte != NewGrupo.TipoReporte ? NewGrupo.TipoReporte : OldGrupo.TipoReporte;

            return OldGrupo;
        }

        [HttpPost]
        public PartialViewResult Detail(ViewGrupoEncuestas Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {                
                return PartialView("_Detail", db.Grupo.Find((Guid)Data.Modelo.Id));
            }
            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Grupo grupo = db.Grupo.Find(id);
                db.Grupo.Remove(grupo);
                db.SaveChanges();

                ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                Guid? Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                return RedirectToRoute("Categorias", Cliente);
            }
        }

        [Route("PreguntasCategoria")]
        [HttpPost]
        public ActionResult PreguntasCategoria(Guid Id)
        {
            return View();
        }
    }
}