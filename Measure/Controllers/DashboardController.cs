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
        [Route("DashboardGeneral/{Id?}")]
        public ActionResult Index(string Id)
        {
            ViewDashboard Model = new ViewDashboard
            {
                ClienteId = Id,
                Clientes = new List<SelectListItem>(),
                Encuestas = new List<SelectListItem>()
            };

            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (Id == Guid.Empty.ToString())
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
                        ClientZero = new Guid(Model.Clientes[1].Value);

                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Clientes = Model.Clientes.Select(s => new SelectListItem
                            {
                                Selected = (s.Value == ClientZero.ToString()),
                                Text = s.Text,
                                Value = s.Value
                            }).ToList();

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
                                                 where A.EncuestaId == EncuestaId
                                                 && A.Resuelta
                                                 select A.UsuarioId.ToString()).Distinct().Count();
                }

                string _Idioma = ((Enums.Idiomas)Usuario.Idioma).ToString();

                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Result.DistPais = (from A in db.UsuariosPorEncuenta
                                       join B in db.Usuario on A.UsuarioId equals B.Id
                                       join C in db.MaestrasDetalle on B.PaisId.ToString() equals C.Valor
                                       where A.EncuestaId == EncuestaId && B.PaisId != null
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
                    Grupos = db.ContenidoPorEncuesta.Where(e => e.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta && e.EncuestaId == EncuestaId).ToList();
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
                                                               where C.GrupoId == _Grupo.Id
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
                                                      where C.GrupoId == _Grupo.Id
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

        [HttpGet]
        [Route("DashboardGeneralDescripcion/{Id?}")]
        public ActionResult Descripcion(string Id)
        {
            ViewDescripcionIndex Model = new ViewDescripcionIndex
            {
                Clientes = new List<SelectListItem>(),
                Encuestas = new List<SelectListItem>(),                
                User = new Usuario()
            };

            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (Id == Guid.Empty.ToString())
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
                        ClientZero = new Guid(Model.Clientes[1].Value);

                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Clientes = Model.Clientes.Select(s => new SelectListItem
                            {
                                Selected = (s.Value == ClientZero.ToString()),
                                Text = s.Text,
                                Value = s.Value
                            }).ToList();

                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == ClientZero)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombre,
                                Value = s.id.ToString()
                            }).ToList());
                        }
                    }


                }
                else
                {
                    Guid ClienteId = new Guid(Id);
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Model.User = db.Usuario.Find(ClienteId);
                    }

                    if (Model.User.RolId == (int)Enums.UserRol.Cliente)
                    {
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == ClienteId)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombre,
                                Value = s.id.ToString()
                            }).ToList());
                        }
                    }
                    else if (Model.User.RolId == (int)Enums.UserRol.Aliado)
                    {
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Encuestas.AddRange(db.Encuesta.Where(e => e.ClienteId == Model.User.ClienteId)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombre,
                                Value = s.id.ToString()
                            }).ToList());
                        }
                    }
                }
            }

            return View(Model);
        }

        [HttpPost]
        public JsonResult BasicTable(string Id)
        {
            Encuesta _Encuesta = new Encuesta();
            List<Usuario> Aliados = new List<Usuario>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Guid EncuestaId = new Guid(Id);
                _Encuesta = db.Encuesta.Find(EncuestaId);
                Aliados = db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Aliado && e.ClienteId == _Encuesta.ClienteId).ToList();
            }

            List<ViewDashboardBasicQuantities> Result = new List<ViewDashboardBasicQuantities>();
            foreach (Usuario Aliado in Aliados)
            {
                List<UsuariosPorEncuenta> Respuestas = new List<UsuariosPorEncuenta>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    List<Guid> Encuestados = db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Encuestado && e.AliadoId == Aliado.Id).OrderBy(o => o.Nombres).Select(s => s.Id).ToList();
                    Respuestas = db.UsuariosPorEncuenta.Where(u => Encuestados.Contains(u.UsuarioId)).ToList();
                }

                Result.Add(new ViewDashboardBasicQuantities
                {
                    Aliado = Aliado,
                    Total = Respuestas.Count(),
                    Completados = Respuestas.Count(r => r.Resuelta),
                    SinCompletar = Respuestas.Count(r => !r.Resuelta)
                });
            }

            return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult BasicDescription(string Id)
        {
            Guid AliadoId = new Guid(Id);
            List<ViewDashboardBasicDescription> Result = new List<ViewDashboardBasicDescription>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Result = (from A in db.Usuario.Where(e => e.AliadoId == AliadoId)
                          join B in db.UsuariosPorEncuenta on A.Id equals B.UsuarioId
                          select new ViewDashboardBasicDescription
                          {
                              Apellidos = A.Apellidos,
                              Correo = A.Correo,
                              Idioma = A.Idioma,
                              IdPais = A.PaisId,
                              FAsignado = B.FechaAsignacion,
                              FCompletado = B.FechaResuelta,    
                              Nombres = A.Nombres
                          }).ToList();
            }

            return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [Route("DashboardAnalitica/{Id?}")]
        public ActionResult AnaliticaIndex(string Id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            ViewAnaliticIndex Model = new ViewAnaliticIndex
            {
                Aliados = new List<SelectListItem>(),
                Clientes = new List<SelectListItem>(),
                PartialIndex = new ViewResponsePartial
                {                    
                    Encuestados = new List<ViewResponsePollUser>()
                }
            };

            if (Login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                Model.PartialIndex.Idioma = Login.Idioma;
                Model.User = new Usuario { RolId = Login.RolId};
                if (string.IsNullOrEmpty(Id))
                {
                    Model.User = new Usuario { RolId = (int)Enums.UserRol.Administrador };
                    Model.Clientes = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } };
                    Model.Aliados = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } };

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
                        ClientZero = new Guid(Model.Clientes[1].Value);

                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {

                            Model.Clientes = Model.Clientes.Select(s => new SelectListItem
                            {
                                Selected = (s.Value == ClientZero.ToString()),
                                Text = s.Text,
                                Value = s.Value
                            }).ToList();

                            Model.Aliados.AddRange(db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Aliado && e.ClienteId == ClientZero)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombres,
                                Value = s.Id.ToString()
                            }).ToList());
                        }
                    }
                }
                else
                {
                    Guid ClienteId = new Guid(Id);

                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Model.User = db.Usuario.Find(ClienteId);
                    }

                    if (Model.User.RolId == (int)Enums.UserRol.Cliente)
                    {
                        Model.Aliados = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } };
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.Aliados.AddRange(db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Aliado && e.ClienteId == ClienteId)
                            .Select(s => new SelectListItem
                            {
                                Text = s.Nombres,
                                Value = s.Id.ToString()
                            }).ToList());
                        }

                    }
                    else if (Model.User.RolId == (int)Enums.UserRol.Aliado)
                    {                        
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Model.PartialIndex.Encuestados = ListarEncuestados(ClienteId);
                        }
                    }
                }
            }
            return View(Model);
        }

        [HttpPost]
        public PartialViewResult ListaDeEncuestados(string IdAliado)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            ViewResponsePartial Result = new ViewResponsePartial
            {
                Idioma = Login.Idioma,
                Encuestados = new List<ViewResponsePollUser>()
            };

            if (!string.IsNullOrEmpty(IdAliado) && !IdAliado.Equals("0"))
            {
                Guid AliadoId = new Guid(IdAliado);

                Result.Encuestados = ListarEncuestados(AliadoId);
            }

            return PartialView("_ListaEncuestados", Result);
        }

        private List<ViewResponsePollUser> ListarEncuestados(Guid AliadoId)
        {
            List<ViewResponsePollUser> Encuestados = new List<ViewResponsePollUser>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Encuestados = (from s in db.Usuario
                               join a in db.UsuariosPorEncuenta on s.Id equals a.UsuarioId
                               where s.AliadoId == AliadoId && a.Resuelta == true
                               select new ViewResponsePollUser
                               {
                                   Id = a.Id,
                                   Correo = s.Correo,
                                   Nombres = s.Nombres,
                                   Apellidos = s.Apellidos,
                                   FechaRespuesta = (DateTime)a.FechaResuelta
                               }).OrderBy(o => o.Correo).ToList();
            }

            return Encuestados;
        }
    }
}