using Measure.Models;
using Measure.ViewModels.Pregunta;
using Measure.ViewModels.PreguntasPorGrupo;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class PreguntasPorGrupoController : Controller
    {
        [Route("PreguntasPorGrupo")]
        public ActionResult Index(Guid GrupoId)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewAsnwerForGroup Modelo = new ViewAsnwerForGroup
            {
                Modelo = new PreguntasPorGrupo
                {
                    GrupoId = GrupoId
                }
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.Group = db.Grupo.Find(GrupoId);
                Modelo.Questions = (from A in db.PreguntasPorGrupo
                                    join B in db.Pregunta on A.PreguntaId equals B.Id
                                    where A.GrupoId == GrupoId && A.Estado
                                    select new ViewAnswerGroup
                                    {
                                        Id = B.Id,
                                        Texto = B.Texto,
                                        Orden = A.Orden,
                                        Idioma = B.Idioma,
                                    }).ToList();
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
                PreguntasPorGrupo contenido = db.PreguntasPorGrupo.Find(Id);
                Guid GrupoId = contenido.GrupoId;

                db.PreguntasPorGrupo.Remove(contenido);
                db.SaveChanges();

                return RedirectToRoute("ContenidoEncuesta", new { GrupoId = GrupoId });
            }
        }

        [HttpPost]
        public ActionResult AddItem(ViewAsnwerForGroup Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Data.Group = db.Grupo.Find(Data.Modelo.GrupoId);
                Data.Questions = (from B in db.Pregunta
                                  where B.ClienteId == Data.Group.ClienteId && B.Estado
                                  select new ViewAnswerGroup
                                  {
                                      Id = B.Id,
                                      Texto = B.Texto,                                      
                                      Idioma = B.Idioma,
                                  }).ToList();
            }

            return View(Data);
        }

        [HttpPost]
        public ActionResult SaveItem(ViewAsnwerForGroup Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }

            PreguntasPorGrupo contenido = new PreguntasPorGrupo
            {
                Estado = Data.Modelo.Estado,
                GrupoId = Data.Modelo.GrupoId,
                Id = Guid.NewGuid(),
                Orden = Data.Modelo.Orden,
                PreguntaId = Data.Modelo.PreguntaId,
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                db.PreguntasPorGrupo.Add(contenido);
                db.SaveChanges();
            }

            return RedirectToRoute("PreguntasPorGrupo", new { GrupoId = contenido.GrupoId });
        }
    }
}