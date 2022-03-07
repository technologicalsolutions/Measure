using Measure.Models;
using Measure.ViewModels.Dashboard;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        [Route("DashboardGeneral/{Id ?}")]
        public ActionResult Index(string Id)
        {
            ViewDashboard Model = new ViewDashboard
            {
                ClienteId = Id,
            };

            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (Id == null)
                {
                    Model.Clientes = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } };
                    Model.Encuestas = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } };

                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Model.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                        .Select(s => new SelectListItem
                        {
                            Text = s.Nombres,
                            Value = s.Id.ToString()
                        }).ToList());
                    }

                    Guid ClientZero = Guid.Empty;
                    if (Model.Clientes.Count(c => c.Value != "0") > 0)
                    {
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Clientes = Model.Clientes.Select(s => new SelectListItem
                            {
                                Selected = (s.Value == Model.Clientes[1].Value),
                                Text = s.Text,
                                Value = s.Value
                            }).ToList();

                            ClientZero = new Guid(Model.Clientes[1].Value);
                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == ClientZero)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombre,
                                Value = s.id.ToString()
                            }).ToList());
                        }
                    }

                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Model.GraphGeneral = (from b in db.Usuario
                                              where b.RolId == (int)Enums.UserRol.Cliente
                                              select new ViewDashboardClientForPoll
                                              {
                                                  Cliente = b.Nombres,
                                                  Encuestas = db.Encuesta.Count(e => e.ClienteId == b.Id)
                                              }).ToList();
                    }

                }
                else
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Guid ClienteId = new Guid(Id);
                        Usuario Cliente = db.Usuario.Find(ClienteId);

                        if (Cliente.RolId == (int)Enums.UserRol.Cliente)
                        {
                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == ClienteId)
                           .Select(s => new SelectListItem
                           {
                               Text = s.Nombre,
                               Value = s.id.ToString()
                           }).ToList());
                        }
                        else if (Cliente.RolId == (int)Enums.UserRol.Aliado)
                        {
                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == Cliente.ClienteId)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombre,
                                Value = s.id.ToString()
                            }).ToList());
                        }

                        Model.GraphGeneral = new List<ViewDashboardClientForPoll>
                        {
                            new ViewDashboardClientForPoll {
                                Cliente = Cliente.Nombres,
                                Encuestas = Model.Encuestas.Count()
                            }
                        };
                    }
                }

            }
            return View(Model);
        }

        [HttpPost]
        [Route("BasicGraph")]
        public JsonResult BasicGraphic(string Id)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;

            ViewDashboardBasicGraphic Result = new ViewDashboardBasicGraphic
            {
                DatosBasicos = new ViewDashboardBasic(),
                DistPais = new List<ViewDashboardDistCount>(),
                RespuestasXGrupo = new List<ViewDashboardBasic>()
            };

            if (Usuario != null)
            {
                Guid EncuestaId = new Guid(Id);                

                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Encuesta _encuesta = db.Encuesta.Find(EncuestaId);
                    Result.DatosBasicos.Label = _encuesta.Nombre;
                }
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Result.DatosBasicos.Max = db.UsuariosPorEncuenta.Count(u => u.EncuestaId == EncuestaId);
                }
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Result.DatosBasicos.Value = (from A in db.UsuariosPorEncuenta
                                                 join B in db.Respuesta on A.UsuarioId equals B.UsuarioId
                                                 where A.EncuestaId == EncuestaId
                                                 select A.UsuarioId.ToString()).Distinct().Count();
                }

                string _Idioma = ((Enums.Idiomas)Usuario.Idioma).ToString();

                using (ModeloEncuesta db = new ModeloEncuesta())
                {                    
                    Result.DistPais = (from A in db.UsuariosPorEncuenta
                                       join B in db.Usuario on A.UsuarioId equals B.Id
                                       join C in db.MaestrasDetalle on B.PaisId equals C.Id
                                       group C by new
                                       {
                                           C.Valor,
                                           C.es_ES,
                                           C.en_US,
                                           C.pt_BR
                                       } into g
                                       select new ViewDashboardDistCount
                                       {
                                           Label =
                                             _Idioma == "es_ES" ? g.Key.es_ES :
                                             _Idioma == "en_US" ? g.Key.en_US :
                                             _Idioma == "pt_BR" ? g.Key.pt_BR : string.Empty,
                                           Value = g.Count(p => p.Valor != null)
                                       }).ToList();
                }

                List<ContenidoPorEncuesta> Grupos = new List<ContenidoPorEncuesta>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Grupos = db.ContenidoPorEncuesta.Where(e => e.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta && e.ComponenteId == EncuestaId).ToList();
                }

                foreach (ContenidoPorEncuesta item in Grupos)
                {                    
                    ViewDashboardBasic Dato = new ViewDashboardBasic();
                    Grupo _Grupo = new Grupo();
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        _Grupo = db.Grupo.Find(item.ComponenteId);

                        int CPreguntas = (from A in db.PreguntasPorGrupo
                                          join B in db.Grupo on A.GrupoId equals B.Id
                                          join C in db.Pregunta on A.PreguntaId equals C.Id
                                          where A.GrupoId == _Grupo.Id
                                          && C.Idioma == Usuario.Idioma
                                          && C.Estado
                                          select A).Count();

                        int CUsuarios = db.UsuariosPorEncuenta.Where(u => u.EncuestaId == EncuestaId).Select(s => s.Id).Distinct().Count();

                        Dato = new ViewDashboardBasic
                        {
                            Label = _Idioma == "es_ES" ? _Grupo.es_Es :
                                    _Idioma == "en_US" ? _Grupo.en_US :
                                    _Idioma == "pt_BR" ? _Grupo.pt_BR : string.Empty,
                            Max = CPreguntas * CUsuarios,
                            Color = _Grupo.Color,
                        };
                    }

                    switch ((Enums.TipoDeControl)_Grupo.TipoReporte)
                    {
                        case Enums.TipoDeControl.ControlAbierto:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                List<string> TempRespuestas = (from A in db.Pregunta
                                                               join B in db.Respuesta on A.Id equals B.PreguntaId
                                                               join C in db.PreguntasPorGrupo on A.Id equals C.PreguntaId
                                                               where C.GrupoId == item.Id
                                                               && A.Idioma == Usuario.Idioma
                                                               && A.Estado
                                                               && !string.IsNullOrEmpty(B.Resultado)
                                                               select B.Resultado).ToList();

                                Dato.Value = TempRespuestas.Count();
                            }
                            Result.RespuestasXGrupo.Add(Dato);
                            break;
                        case Enums.TipoDeControl.ControlBooleano:
                            break;
                        case Enums.TipoDeControl.ControlCerrado:
                            break;
                        case Enums.TipoDeControl.ControlMatriz:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                int TempRespuestas = (from A in db.Pregunta
                                                      join B in db.Respuesta on A.Id equals B.PreguntaId
                                                      join C in db.PreguntasPorGrupo on A.Id equals C.PreguntaId
                                                      where C.GrupoId == item.Id
                                                      && A.Idioma == Usuario.Idioma
                                                      && A.Estado
                                                      select B.Resultado).Count();

                                Dato.Value = TempRespuestas;
                            }
                            Result.RespuestasXGrupo.Add(Dato);
                            break;
                    }

                }

            }

            return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}