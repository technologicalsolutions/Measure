using Measure.Models;
using Measure.ViewModels.ContenidoPorEncuesta;
using Measure.ViewModels.Grupo;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class ContenidoEncuestaController : Controller
    {
        [Route("ContenidoEncuesta")]        
        public ActionResult Index(Guid EncuestaId)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewContenidosEncuesta Modelo = new ViewContenidosEncuesta
            {
                Lista = new List<ViewSurveyContent>(),
                TiposComponente = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Inicio", Value = ((int)Enums.TipoComponente.Inicio).ToString() },
                    new SelectListItem { Text = "Categoria", Value =((int)Enums.TipoComponente.CategoriaEncuesta).ToString() },
                    new SelectListItem { Text = "Fin", Value = ((int)Enums.TipoComponente.Fin).ToString() },
                    new SelectListItem { Text = "Grabar", Value =((int)Enums.TipoComponente.Grabar).ToString() },
                }
            };

            List<ContenidoPorEncuesta> TempList = new List<ContenidoPorEncuesta>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.Padre = db.Encuesta.Find(EncuestaId);
                TempList = db.ContenidoPorEncuesta.Where(c => c.EncuestaId == EncuestaId && c.ComponenteId != Guid.Empty).OrderBy(o => o.Orden).ToList();
            }

            List<Guid> TempCont = TempList.Where(c => c.TipoComponente != (int)Enums.TipoComponente.CategoriaEncuesta).Select(c => c.ComponenteId).ToList();
            List<Guid> TempGroup = TempList.Where(c => c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta).Select(c => c.ComponenteId).ToList();

            List<Contenido> Contents = new List<Contenido>();
            List<Grupo> Groups = new List<Grupo>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Contents = db.Contenido.Where(c => TempCont.Contains(c.Id)).ToList();
                Groups = db.Grupo.Where(c => TempGroup.Contains(c.Id)).ToList();
            }

            foreach (ContenidoPorEncuesta item in TempList)
            {
                ViewSurveyContent addItem = new ViewSurveyContent
                {
                    ComponenteId = item.ComponenteId,
                    EncuestaId = item.EncuestaId,
                    Estado = item.Estado,
                    Id = item.Id,
                    Orden = item.Orden,
                    TipoComponente = item.TipoComponente,
                    TipoDeComponente = ((Enums.TipoComponente)item.TipoComponente).ToString(),
                };

                if (item.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta)
                {
                    Grupo grupo = Groups.FirstOrDefault(g => g.Id == item.ComponenteId);
                    addItem.NombreComponente = Login.Idioma == (int)Enums.Idiomas.es_ES ? grupo.es_Es : Login.Idioma == (int)Enums.Idiomas.en_US ? grupo.en_US : grupo.pt_BR;
                }
                else
                {
                    Contenido contenido = Contents.FirstOrDefault(g => g.Id == item.ComponenteId);
                    if (contenido != null)
                    {
                        addItem.NombreComponente = !string.IsNullOrEmpty(contenido.es_Es) ? contenido.es_Es : !string.IsNullOrEmpty(contenido.en_US) ? contenido.en_US : contenido.pt_BR;
                    }                    
                }

                Modelo.Lista.Add(addItem);
            }

            return View(Modelo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid Id)
        {            
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ContenidoPorEncuesta contenido = db.ContenidoPorEncuesta.Find(Id);
                Guid EncuestaId = contenido.EncuestaId;

                db.ContenidoPorEncuesta.Remove(contenido);
                db.SaveChanges();

                return RedirectToRoute("ContenidoEncuesta", new { EncuestaId = EncuestaId });
            }
        }

        [HttpPost]
        public ActionResult AddItem(ViewSurveyContent Data)
        {
            string view = "Item";
            ViewContentList Modelo = new ViewContentList
            {
                Grupos = new List<ViewContentGroup>(),
                Modelo = new ContenidoPorEncuesta
                {                    
                    EncuestaId = Data.EncuestaId,
                    TipoComponente = Data.TipoComponente,                    
                },              
            };

            if (Data.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta)
            {
                view += "Group";

                List<Grupo> Grupos = new List<Grupo>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Grupos = db.Grupo.Where(c => c.ClienteId == Data.ClienteId && c.Estado).ToList();
                    Modelo.Padre = db.Encuesta.Find(Data.EncuestaId);
                }
                List<Guid> TempGrupos = Grupos.Select(c => c.Id).ToList();

                List<PreguntasPorGrupo> Preguntas = new List<PreguntasPorGrupo>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Preguntas = db.PreguntasPorGrupo.Where(p => TempGrupos.Contains(p.GrupoId)).ToList();
                }

                foreach (Grupo item in Grupos)
                {
                    ViewContentGroup AddItem = new ViewContentGroup
                    {
                        CantidadPreguntas = Preguntas.Count(p => p.GrupoId == item.Id),                                         
                        en_US = item.en_US,                        
                        es_Es = item.es_Es,
                        Id = item.Id,
                        pt_BR = item.pt_BR,                        
                    };

                    Modelo.Grupos.Add(AddItem);
                }
            }
            else
            {
                view += "Content";
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Modelo.Contenidos = db.Contenido.Where(c => c.ClienteId == Data.ClienteId && c.Estado).ToList();
                    Modelo.Padre = db.Encuesta.Find(Data.EncuestaId);
                }
            }

            return View(view, Modelo);
        }

        [HttpPost]
        public ActionResult SaveItem(ViewContentList Data)
        {            
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            ContenidoPorEncuesta contenido = new ContenidoPorEncuesta
            {
                Id = Guid.NewGuid(),
                ComponenteId = Data.Modelo.ComponenteId,
                EncuestaId = Data.Modelo.EncuestaId,
                Estado = Data.Modelo.Estado,
                Orden = Data.Modelo.Orden,
                TipoComponente = Data.Modelo.TipoComponente,
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {                
                db.ContenidoPorEncuesta.Add(contenido);
                db.SaveChanges();
            }

            return RedirectToRoute("ContenidoEncuesta", new { EncuestaId = contenido.EncuestaId });

        }
    }
}