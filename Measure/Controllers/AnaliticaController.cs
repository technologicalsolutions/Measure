using Measure.Models;
using Measure.ViewModels.Analitic;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class AnaliticaController : Controller
    {
        [Route("Analitica")]
        public ActionResult Index()
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
            if (Login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewIndex Model = new ViewIndex
            {
                Encuestas = new List<SelectListItem>(),
                Grupos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione..." } },
                Modelo = new ViewAnalitic(),
                Parametros = new List<ViewAnalitic>()
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Model.Encuestas.AddRange(db.Encuesta.Where(e => e.Estado).Select(s => new SelectListItem { Value = s.id.ToString(), Text = s.Nombre }).ToList());
            }

            if (Model.Encuestas.Count() > 0)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Guid IdEncuesta = new Guid(Model.Encuestas[0].Value);
                    List<Guid> temp = db.ContenidoPorEncuesta.Where(c => c.EncuestaId == IdEncuesta && c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta).Select(s => s.ComponenteId).ToList();
                    Model.Grupos.AddRange(db.Grupo.Where(g => temp.Contains(g.Id))
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = (Login.Idioma == (int)Enums.Idiomas.es_ES ? s.es_Es : Login.Idioma == (int)Enums.Idiomas.en_US ? s.en_US : s.pt_BR)
                    }).ToList());
                }
            }

            return View(Model);
        }

        [HttpPost]
        public PartialViewResult PartialPage(string EncuestaId, string GrupoId, int Idioma)
        {
            List<ViewAnalitic> Modelo = new List<ViewAnalitic>();
            
            if (string.IsNullOrEmpty(EncuestaId) && string.IsNullOrEmpty(GrupoId))
            {                
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Modelo = db.Analitica.Select(s => new ViewAnalitic
                    {
                        Alcance = s.Alcance,
                        CalificacionConceptual = s.CalificacionConceptual,
                        CalificacionMatematica = s.CalificacionMatematica,
                        DescripcionActividadesEm = s.DescripcionActividadesEm,
                        DescripcionActividadesMa = s.DescripcionActividadesMa,
                        DescripcionRiesgos = s.DescripcionRiesgos,
                        Dimensiones = s.Dimensiones,
                        Estado = s.Estado,
                        GrupoId = s.GrupoId,
                        GrupoName = Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                        Id = s.Id,
                        MadurezConceptual = s.MadurezConceptual,
                        MadurezMatematica = s.MadurezMatematica,
                        SubTituloGrafico = s.SubTituloGrafico,
                        TipoParametroId = s.TipoParametroId,
                        TituloGrafico = s.TituloGrafico,
                        ValorMaximo = s.ValorMaximo,
                        ValorMinimo = s.ValorMinimo,
                        Variable1 = s.Variable1,
                        Variable2 = s.Variable2,
                    }).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(EncuestaId) && string.IsNullOrEmpty(GrupoId))
                {
                    Guid IdEncuesta = new Guid(EncuestaId);
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        List<Guid> temp = db.ContenidoPorEncuesta.Where(c => c.EncuestaId == IdEncuesta && c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta).Select(s => s.ComponenteId).ToList();
                        Modelo = db.Analitica.Where(a => temp.Contains((Guid)a.GrupoId)).Select(s => new ViewAnalitic
                        {
                            Alcance = s.Alcance,
                            CalificacionConceptual = s.CalificacionConceptual,
                            CalificacionMatematica = s.CalificacionMatematica,
                            DescripcionActividadesEm = s.DescripcionActividadesEm,
                            DescripcionActividadesMa = s.DescripcionActividadesMa,
                            DescripcionRiesgos = s.DescripcionRiesgos,
                            Dimensiones = s.Dimensiones,
                            Estado = s.Estado,
                            GrupoId = s.GrupoId,
                            GrupoName = Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                            Id = s.Id,
                            MadurezConceptual = s.MadurezConceptual,
                            MadurezMatematica = s.MadurezMatematica,
                            SubTituloGrafico = s.SubTituloGrafico,
                            TipoParametroId = s.TipoParametroId,
                            TituloGrafico = s.TituloGrafico,
                            ValorMaximo = s.ValorMaximo,
                            ValorMinimo = s.ValorMinimo,
                            Variable1 = s.Variable1,
                            Variable2 = s.Variable2,
                        }).ToList();
                    }
                }
                else if (string.IsNullOrEmpty(EncuestaId) && !string.IsNullOrEmpty(GrupoId))
                {
                    Guid IdGrupo = new Guid(GrupoId);
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Modelo = db.Analitica.Where(a => a.GrupoId == IdGrupo).Select(s => new ViewAnalitic
                        {
                            Alcance = s.Alcance,
                            CalificacionConceptual = s.CalificacionConceptual,
                            CalificacionMatematica = s.CalificacionMatematica,
                            DescripcionActividadesEm = s.DescripcionActividadesEm,
                            DescripcionActividadesMa = s.DescripcionActividadesMa,
                            DescripcionRiesgos = s.DescripcionRiesgos,
                            Dimensiones = s.Dimensiones,
                            Estado = s.Estado,
                            GrupoId = s.GrupoId,
                            GrupoName = Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                            Id = s.Id,
                            MadurezConceptual = s.MadurezConceptual,
                            MadurezMatematica = s.MadurezMatematica,
                            SubTituloGrafico = s.SubTituloGrafico,
                            TipoParametroId = s.TipoParametroId,
                            TituloGrafico = s.TituloGrafico,
                            ValorMaximo = s.ValorMaximo,
                            ValorMinimo = s.ValorMinimo,
                            Variable1 = s.Variable1,
                            Variable2 = s.Variable2,
                        }).ToList();
                    }
                }
                else if (!string.IsNullOrEmpty(EncuestaId) && !string.IsNullOrEmpty(GrupoId))
                {                    
                    Guid _Grupo = new Guid(GrupoId);

                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {                        
                        Modelo = db.Analitica.Where(a => a.GrupoId == _Grupo).Select(s => new ViewAnalitic
                        {
                            Alcance = s.Alcance,
                            CalificacionConceptual = s.CalificacionConceptual,
                            CalificacionMatematica = s.CalificacionMatematica,
                            DescripcionActividadesEm = s.DescripcionActividadesEm,
                            DescripcionActividadesMa = s.DescripcionActividadesMa,
                            DescripcionRiesgos = s.DescripcionRiesgos,
                            Dimensiones = s.Dimensiones,
                            Estado = s.Estado,
                            GrupoId = s.GrupoId,
                            GrupoName = Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                            Id = s.Id,
                            MadurezConceptual = s.MadurezConceptual,
                            MadurezMatematica = s.MadurezMatematica,
                            SubTituloGrafico = s.SubTituloGrafico,
                            TipoParametroId = s.TipoParametroId,
                            TituloGrafico = s.TituloGrafico,
                            ValorMaximo = s.ValorMaximo,
                            ValorMinimo = s.ValorMinimo,
                            Variable1 = s.Variable1,
                            Variable2 = s.Variable2,
                        }).ToList();
                    }
                }
            }
            
            return PartialView("_PartialPage", Modelo);
        }

        public ActionResult CreateOrEdit(int? Id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
            if (Login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewIndex Model = new ViewIndex
            {
                Encuestas = new List<SelectListItem>(),
                Grupos = new List<SelectListItem> { new SelectListItem { Text = "Seleccione..." } },
                Modelo = new ViewAnalitic(),
                Parametros = new List<ViewAnalitic>()
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Model.Encuestas.AddRange(db.Encuesta.Where(e => e.Estado).Select(s => new SelectListItem { Value = s.id.ToString(), Text = s.Nombre }).ToList());
            }

            if (Model.Encuestas.Count() > 0)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Guid IdEncuesta = new Guid(Model.Encuestas[0].Value);
                    List<Guid> temp = db.ContenidoPorEncuesta.Where(c => c.EncuestaId == IdEncuesta && c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta).Select(s => s.ComponenteId).ToList();
                    Model.Grupos.AddRange(db.Grupo.Where(g => temp.Contains(g.Id))
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = (Login.Idioma == (int)Enums.Idiomas.es_ES ? s.es_Es : Login.Idioma == (int)Enums.Idiomas.en_US ? s.en_US : s.pt_BR)
                    }).ToList());
                }
            }

            if (Id == null)
            {
                ViewBag.Title = "Crear parametro";                
            }
            else
            {
                ViewBag.Title = "Editar parametro";                

                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Analitica s = db.Analitica.Find(Id);
                    Model.Modelo = new ViewAnalitic
                    {
                        Alcance = s.Alcance,
                        CalificacionConceptual = s.CalificacionConceptual,
                        CalificacionMatematica = s.CalificacionMatematica,
                        DescripcionActividadesEm = s.DescripcionActividadesEm,
                        DescripcionActividadesMa = s.DescripcionActividadesMa,
                        DescripcionRiesgos = s.DescripcionRiesgos,
                        Dimensiones = s.Dimensiones,
                        Estado = s.Estado,
                        GrupoId = s.GrupoId,
                        GrupoName = Login.Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Login.Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                        Id = s.Id,
                        MadurezConceptual = s.MadurezConceptual,
                        MadurezMatematica = s.MadurezMatematica,
                        SubTituloGrafico = s.SubTituloGrafico,
                        TipoParametroId = s.TipoParametroId,
                        TituloGrafico = s.TituloGrafico,
                        ValorMaximo = s.ValorMaximo,
                        ValorMinimo = s.ValorMinimo,
                        Variable1 = s.Variable1,
                        Variable2 = s.Variable2,
                    };

                    if (s.GrupoId != null)
                    {
                        ContenidoPorEncuesta Contenido = db.ContenidoPorEncuesta.FirstOrDefault(c => c.ComponenteId == s.GrupoId);
                        Model.Encuestas = Model.Encuestas.Select(n => new SelectListItem { Selected = (n.Value == Contenido.EncuestaId.ToString()), Text = n.Text, Value = n.Value }).ToList();
                        Model.Grupos = Model.Grupos.Select(n => new SelectListItem { Selected = (n.Value == s.GrupoId.ToString()), Text = n.Text, Value = n.Value }).ToList();
                    }
                }
            }
            return View(Model);
        }

        public ActionResult Save(ViewAnalitic Data)
        {
            if (Data.Id == 0)
            {
                Analitica analitica = new Analitica
                {
                    Alcance = Data.Alcance,
                    CalificacionConceptual = Data.CalificacionConceptual,
                    CalificacionMatematica = Data.CalificacionMatematica,
                    DescripcionActividadesEm = Data.DescripcionActividadesEm,
                    DescripcionActividadesMa = Data.DescripcionActividadesMa,
                    DescripcionRiesgos = Data.DescripcionRiesgos,
                    Dimensiones = Data.Dimensiones,
                    Estado = Data.Estado,
                    GrupoId = Data.GrupoId,
                    MadurezConceptual = Data.MadurezConceptual,
                    MadurezMatematica = Data.MadurezMatematica,
                    SubTituloGrafico = Data.SubTituloGrafico,
                    TipoParametroId = Data.TipoParametroId,
                    TituloGrafico = Data.TituloGrafico,
                    ValorMaximo = Data.ValorMaximo,
                    ValorMinimo = Data.ValorMinimo,
                    Variable1 = Data.Variable1,
                    Variable2 = Data.Variable2,                    
                };

                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    db.Analitica.Add(analitica);
                    db.SaveChanges();
                }
            }
            else
            {
                Analitica EditAnalitica = new Analitica();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    EditAnalitica = db.Analitica.Find(Data.Id);
                }

                EditAnalitica.Alcance = EditAnalitica.Alcance != Data.Alcance ? Data.Alcance: EditAnalitica.Alcance;
                EditAnalitica.CalificacionConceptual = EditAnalitica.CalificacionConceptual != Data.CalificacionConceptual ?  Data.CalificacionConceptual: EditAnalitica.CalificacionConceptual;
                EditAnalitica.CalificacionMatematica = EditAnalitica.CalificacionMatematica != Data.CalificacionMatematica ?  Data.CalificacionMatematica: EditAnalitica.CalificacionConceptual;
                EditAnalitica.DescripcionActividadesEm = EditAnalitica.DescripcionActividadesEm != Data.DescripcionActividadesEm ?  Data.DescripcionActividadesEm : EditAnalitica.DescripcionActividadesEm;
                EditAnalitica.DescripcionActividadesMa = EditAnalitica.DescripcionActividadesMa != Data.DescripcionActividadesMa ?  Data.DescripcionActividadesMa : EditAnalitica.DescripcionActividadesMa;
                EditAnalitica.DescripcionRiesgos = EditAnalitica.DescripcionRiesgos != Data.DescripcionRiesgos ?  Data.DescripcionRiesgos : EditAnalitica.DescripcionRiesgos;
                EditAnalitica.Dimensiones = EditAnalitica.Dimensiones != Data.Dimensiones ?  Data.Dimensiones : EditAnalitica.Dimensiones;
                EditAnalitica.Estado = EditAnalitica.Estado != Data.Estado ?  Data.Estado : EditAnalitica.Estado;
                EditAnalitica.GrupoId = EditAnalitica.GrupoId != Data.GrupoId ?  Data.GrupoId: EditAnalitica.GrupoId;
                EditAnalitica.MadurezConceptual = EditAnalitica.MadurezConceptual != Data.MadurezConceptual ?  Data.MadurezConceptual : EditAnalitica.MadurezConceptual;
                EditAnalitica.MadurezMatematica = EditAnalitica.MadurezMatematica != Data.MadurezMatematica ?  Data.MadurezMatematica : EditAnalitica.MadurezMatematica;
                EditAnalitica.SubTituloGrafico = EditAnalitica.SubTituloGrafico != Data.SubTituloGrafico ?  Data.SubTituloGrafico : EditAnalitica.SubTituloGrafico;
                EditAnalitica.TipoParametroId = EditAnalitica.TipoParametroId != Data.TipoParametroId ?  Data.TipoParametroId: EditAnalitica.TipoParametroId;
                EditAnalitica.TituloGrafico = EditAnalitica.TituloGrafico != Data.TituloGrafico ?  Data.TituloGrafico: EditAnalitica.TituloGrafico;
                EditAnalitica.ValorMaximo = EditAnalitica.ValorMaximo != Data.ValorMaximo ?  Data.ValorMaximo: EditAnalitica.ValorMaximo;
                EditAnalitica.ValorMinimo = EditAnalitica.ValorMinimo != Data.ValorMinimo ?  Data.ValorMinimo: EditAnalitica.ValorMinimo;
                EditAnalitica.Variable1 = EditAnalitica.Variable1 != Data.Variable1 ?  Data.Variable1: EditAnalitica.Variable1;
                EditAnalitica.Variable2 = EditAnalitica.Variable2 != Data.Variable2 ?  Data.Variable2: EditAnalitica.Variable2;

                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    db.Entry(EditAnalitica).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToRoute("Analitica");
        }

        [HttpPost]
        public PartialViewResult Detail(long Id, int Idioma)
        {
            ViewAnalitic Modelo = new ViewAnalitic();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Analitica s = db.Analitica.Find(Id);
                Modelo = new ViewAnalitic
                {
                    Alcance = s.Alcance,
                    CalificacionConceptual = s.CalificacionConceptual,
                    CalificacionMatematica = s.CalificacionMatematica,
                    DescripcionActividadesEm = s.DescripcionActividadesEm,
                    DescripcionActividadesMa = s.DescripcionActividadesMa,
                    DescripcionRiesgos = s.DescripcionRiesgos,
                    Dimensiones = s.Dimensiones,
                    Estado = s.Estado,
                    GrupoId = s.GrupoId,
                    GrupoName = Idioma == (int)Enums.Idiomas.es_ES ? s.Grupo.es_Es : Idioma == (int)Enums.Idiomas.en_US ? s.Grupo.en_US : s.Grupo.pt_BR,
                    Id = s.Id,
                    MadurezConceptual = s.MadurezConceptual,
                    MadurezMatematica = s.MadurezMatematica,
                    SubTituloGrafico = s.SubTituloGrafico,
                    TipoParametroId = s.TipoParametroId,
                    TituloGrafico = s.TituloGrafico,
                    ValorMaximo = s.ValorMaximo,
                    ValorMinimo = s.ValorMinimo,
                    Variable1 = s.Variable1,
                    Variable2 = s.Variable2,
                };
            }
            return PartialView("_Detail", Modelo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long Id)
        {
            ViewLogin _Usuario = HttpContext.Session["login"] as ViewLogin;
            if (_Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Analitica analitica = db.Analitica.Find(Id);

                db.Analitica.Remove(analitica);
                db.SaveChanges();
            }

            return RedirectToRoute("Analitica");
        }
    }
}