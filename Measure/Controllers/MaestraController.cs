using Measure.Enums;
using Measure.Models;
using Measure.ViewModels.Maestra;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class MaestraController : Controller
    {
        [Route("Maestras")]
        public ActionResult Index(Guid? Cliente)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewLogin login = HttpContext.Session["login"] as ViewLogin;

            ViewMaster Modelo = new ViewMaster
            {
                Acciones = DbAcciones.Crea,
                Modelo = new Maestras(),
                Clientes = new List<SelectListItem>(),
                Lista = new List<Maestras>()
            };

            List<Maestras> Lista = new List<Maestras>();
            if (Cliente != Guid.Empty)
            {
                Modelo.Modelo.ClienteId = (Guid)Cliente;
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Lista = db.Maestras.Where(a => a.ClienteId == Modelo.Modelo.ClienteId).ToList();
                }
            }
            else
            {
                if (login.RolId == (int)UserRol.Administrador)
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        List<string> Excludes = new List<string> { "Pais", "Idioma" };
                        Lista = db.Maestras.Where(m => !Excludes.Contains(m.es_ES)).ToList();
                        Modelo.Clientes = db.Usuario.Where(u => u.RolId == (int)UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                    }
                }
                else
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Lista = db.Maestras.Where(m => m.ClienteId != Guid.Empty).ToList();
                        Modelo.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                    }
                }
                
            }

            foreach (Maestras item in Lista)
            {
                Modelo.Lista.Add(new Maestras
                {
                    Activo = item.Activo,
                    Base = item.Base,
                    en_US = item.en_US,
                    es_ES = item.es_ES,
                    ClienteId = item.ClienteId,
                    Id = item.Id,
                    pt_BR = item.pt_BR,
                });
            }

            return View(Modelo);
        }

        public ActionResult CreateOrEdit(ViewMaster Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                switch (Data.Acciones)
                {
                    case DbAcciones.Crea:
                        Data.Modelo.Base = false;
                        db.Maestras.Add(Data.Modelo);
                        db.SaveChanges();
                        break;
                    case DbAcciones.Actualiza:
                        Maestras _old = db.Maestras.Where(m => m.Id == Data.Modelo.Id).FirstOrDefault();
                        _old.Activo = _old.Activo != Data.Modelo.Activo ? Data.Modelo.Activo : _old.Activo;
                        _old.en_US = _old.en_US != Data.Modelo.en_US ? Data.Modelo.en_US : _old.en_US;
                        _old.es_ES = _old.es_ES != Data.Modelo.es_ES ? Data.Modelo.es_ES : _old.es_ES;
                        _old.pt_BR = _old.pt_BR != Data.Modelo.pt_BR ? Data.Modelo.pt_BR : _old.pt_BR;

                        db.Entry(_old).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                }
            }

            return RedirectToRoute("Maestras");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Maestras _Maestra = db.Maestras.Find(Id);
                db.Maestras.Remove(_Maestra);
                db.SaveChanges();
            }
            return RedirectToRoute("Maestras");
        }

        [Route("Detalle/{id}")]
        public ActionResult Detalles(int Id)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewLogin login = HttpContext.Session["login"] as ViewLogin;

            ViewDetailsMaster Modelo = new ViewDetailsMaster
            {
                Acciones = DbAcciones.Crea,                
                Modelo = new MaestrasDetalle
                {
                    MaestraId = Id
                },
                Lista = new List<MaestrasDetalle>()
            };

            List<MaestrasDetalle> Lista = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Maestras _Maestra = db.Maestras.Find(Id);
                Modelo.Maestra = new Maestras
                {
                    Base = _Maestra.Base,
                    ClienteId = _Maestra.ClienteId,
                    es_ES = _Maestra.es_ES,
                    en_US = _Maestra.en_US,
                    Id = Id,
                    pt_BR = _Maestra.pt_BR,
                    Activo = _Maestra.Activo,                    
                };
                Lista = _Maestra.MaestrasDetalle.ToList();
            }

            foreach (var item in Lista)
            {
                Modelo.Lista.Add(new MaestrasDetalle
                {
                    en_US = item.en_US,
                    es_ES = item.es_ES,
                    Estado = item.Estado,
                    pt_BR = item.pt_BR,
                    Id = item.Id,
                    MaestraId = item.MaestraId,
                    Valor = item.Valor,
                });
            }

            return View(Modelo);
        }

        public ActionResult CreateOrEditDetail(ViewDetailsMaster Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                switch (Data.Acciones)
                {
                    case DbAcciones.Crea:                        
                        db.MaestrasDetalle.Add(Data.Modelo);
                        db.SaveChanges();
                        break;
                    case DbAcciones.Actualiza:
                        MaestrasDetalle _old = db.MaestrasDetalle.Where(m => m.Id == Data.Modelo.Id).FirstOrDefault();

                        _old.en_US = _old.en_US != Data.Modelo.en_US ? Data.Modelo.en_US : _old.en_US;
                        _old.es_ES = _old.es_ES != Data.Modelo.es_ES ? Data.Modelo.es_ES : _old.es_ES;
                        _old.pt_BR = _old.pt_BR != Data.Modelo.pt_BR ? Data.Modelo.pt_BR : _old.pt_BR;                        
                        _old.Estado = _old.Estado != Data.Modelo.Estado ? Data.Modelo.Estado : _old.Estado;

                        db.Entry(_old).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                }
            }

            return RedirectToRoute("Detalle", new { Id = Data.Modelo.MaestraId });
        }       

        [Route("Paises")]
        public ActionResult Paises()
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewDetailsMaster Modelo = new ViewDetailsMaster
            {
                Acciones = DbAcciones.Crea,              
                Lista = new List<MaestrasDetalle>()
            };

            List<MaestrasDetalle> Lista = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Maestras _Maestra = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais"));
                Lista = _Maestra.MaestrasDetalle.ToList();

                Modelo.Maestra = new Maestras
                {
                    Base = _Maestra.Base,
                    ClienteId = _Maestra.ClienteId,
                    es_ES = _Maestra.es_ES,
                    en_US = _Maestra.en_US,
                    Id = _Maestra.Id,
                    pt_BR = _Maestra.pt_BR,
                    Activo = _Maestra.Activo,
                };

                Modelo.Modelo = new MaestrasDetalle
                {
                    Id = _Maestra.Id,
                };
            }

            foreach (MaestrasDetalle item in Lista)
            {
                Modelo.Lista.Add(new MaestrasDetalle
                {
                    en_US = item.en_US,
                    es_ES = item.es_ES,
                    Estado = item.Estado,
                    pt_BR = item.pt_BR,
                    Id = item.Id,
                    MaestraId = item.MaestraId,
                    Valor = item.Valor,
                });
            }

            return View(Modelo);
        }

        [Route("Idiomas")]
        public ActionResult Idiomas()
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewDetailsMaster Modelo = new ViewDetailsMaster
            {
                Acciones = DbAcciones.Crea,
                Lista = new List<MaestrasDetalle>()
            };

            List<MaestrasDetalle> Lista = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Maestras _Maestra = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma"));
                Lista = _Maestra.MaestrasDetalle.ToList();

                Modelo.Maestra = new Maestras
                {
                    Base = _Maestra.Base,
                    ClienteId = _Maestra.ClienteId,
                    es_ES = _Maestra.es_ES,
                    en_US = _Maestra.en_US,
                    Id = _Maestra.Id,
                    pt_BR = _Maestra.pt_BR,
                    Activo = _Maestra.Activo,
                };

                Modelo.Modelo = new MaestrasDetalle
                {
                    Id = _Maestra.Id,
                };
            }

            foreach (MaestrasDetalle item in Lista)
            {
                Modelo.Lista.Add(new MaestrasDetalle
                {
                    en_US = item.en_US,
                    es_ES = item.es_ES,
                    pt_BR = item.pt_BR,
                    Id = item.Id,
                    MaestraId = item.MaestraId,
                    Valor = item.Valor,
                });
            }

            return View(Modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDetail(int Id)
        {
            int MaestraId = 0;
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                MaestrasDetalle _Maestra = db.MaestrasDetalle.Find(Id);
                MaestraId = _Maestra.MaestraId;
                db.MaestrasDetalle.Remove(_Maestra);
                db.SaveChanges();
            }
            return RedirectToRoute("Detalle", new { Id = MaestraId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InactiveDetail(int Id)
        {
            int MaestraId = 0;
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                MaestrasDetalle _Maestra = db.MaestrasDetalle.Find(Id);
                _Maestra.Estado = false;

                MaestraId = _Maestra.MaestraId;
                db.Entry(_Maestra).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToRoute("Detalle", new { Id = MaestraId });
        }
    }
}