using Measure.Enums;
using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Analitic;
using Measure.ViewModels.Dashboard;
using Measure.ViewModels.Usuario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class DashboardController : Controller
    {
        private readonly Random _random = new Random();

        [HttpGet]
        [Route("DashboardDataGeneral/{Id?}")]
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
        [Route("DashboardDataGeneralDescripcion/{Id?}")]
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
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
            List<ViewDashboardBasicQuantities> Result = new List<ViewDashboardBasicQuantities>();
            if (Login != null)
            {
                Encuesta _Encuesta = new Encuesta();
                List<Usuario> Aliados = new List<Usuario>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Guid EncuestaId = new Guid(Id);
                    _Encuesta = db.Encuesta.Find(EncuestaId);
                    if (Login.RolId != (int)Enums.UserRol.Aliado)
                    {
                        Aliados = db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Aliado && e.ClienteId == _Encuesta.ClienteId).ToList();
                    }
                    else
                    {
                        Aliados = db.Usuario.Where(e => e.RolId == (int)Enums.UserRol.Aliado && e.ClienteId == _Encuesta.ClienteId && e.Id == Login.Id).ToList();
                    }
                }

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
                Paises = new List<SelectListItem>(),
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
                Model.User = new Usuario { RolId = Login.RolId };

                List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.Where(d => d.Estado).ToList();
                }
                Model.Paises = Paises.Select(s => new SelectListItem
                {
                    Text = Login.Idioma == (int)Enums.Idiomas.es_ES ? s.es_ES : Login.Idioma == (int)Enums.Idiomas.en_US ? s.en_US : s.pt_BR,
                    Value = s.Valor
                }).OrderBy(o => o.Text).ToList();

                if (Id == Guid.Empty.ToString())
                {
                    Model.User = new Usuario { RolId = (int)Enums.UserRol.Administrador };
                    Model.Encuestas = new List<SelectListItem>();
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
                    if (Model.Clientes.Count() > 1)
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
                            Model.PartialIndex.Encuestados = ListarEncuestados(ClienteId, null);
                        }
                    }
                }
            }
            return View(Model);
        }

        [HttpPost]
        [Route("GenerarAnalisis")]
        public ActionResult GenerarAnalisis(string Data)
        {
            ViewDataAnalitic DatosBrutos = JsonConvert.DeserializeObject<ViewDataAnalitic>(Data);
            ViewAnaliticSquare Modelo = new ViewAnaliticSquare();

            int totalGraficos = 0;
            if ((TipoAnalisis)DatosBrutos.Filtros.TipoDeAnalisis == TipoAnalisis.CuadroUno)
            {
                List<ViewDataAnaliticDetail> DataBase = (from a in DatosBrutos.Encuestados
                                                         where ((string.IsNullOrEmpty(DatosBrutos.Filtros.Pais)) || (a.Pais.Equals(DatosBrutos.Filtros.Pais)))
                                                         && ((string.IsNullOrEmpty(DatosBrutos.Filtros.Sucursal)) || (a.Sucursal.Equals(DatosBrutos.Filtros.Sucursal)))
                                                         && ((string.IsNullOrEmpty(DatosBrutos.Filtros.Gerencia)) || (a.Gerencia.Equals(DatosBrutos.Filtros.Gerencia)))
                                                         && ((string.IsNullOrEmpty(DatosBrutos.Filtros.Rol)) || (a.Rol.Equals(DatosBrutos.Filtros.Rol)))
                                                         select a).ToList();

                Modelo.CuadroUno = ViewAnaliticSquareOne(DatosBrutos.EncuestaId, DataBase, DatosBrutos.Encuestados);
                totalGraficos = Modelo.CuadroUno.Count();
            }
            else if ((TipoAnalisis)DatosBrutos.Filtros.TipoDeAnalisis == TipoAnalisis.CuadroDos)
            {
                List<ViewDataAnaliticDetail> DataBase = (from a in DatosBrutos.Encuestados
                                                         where ((string.IsNullOrEmpty(DatosBrutos.Filtros.Pais)) || (a.Pais.Equals(DatosBrutos.Filtros.Pais)))
                                                         && ((string.IsNullOrEmpty(DatosBrutos.Filtros.Sucursal)) || (a.Sucursal.Equals(DatosBrutos.Filtros.Sucursal)))
                                                         select a).ToList();

                Modelo.CuadroDos = CuadroDosGrafico(DatosBrutos.EncuestaId, DataBase);
                totalGraficos = 2 + Modelo.CuadroDos.GrafBase.Count();
            }

            string TituloGeneral = "Graficos ";
            List<string> TituloFiltro = new List<string>();
            foreach (var property in DatosBrutos.Filtros.GetType().GetProperties())
            {
                if (property.GetValue(DatosBrutos.Filtros) != null)
                {
                    TituloFiltro.Add($"{property.Name}: {property.GetValue(DatosBrutos.Filtros)}");
                }
            }

            if (TituloFiltro.Count() > 0)
            {


                TituloGeneral += $"Filtrados, {string.Join(", ", TituloFiltro)}, Cantidad de Graficos: {totalGraficos}.";
            }
            else
            {
                TituloGeneral += $"Generales, Cantidad de Graficos: {Modelo.CuadroUno.Count()}.";
            }

            Modelo.TituloGeneral = TituloGeneral;

            return View(Modelo);
        }

        private List<GraphicBase> ViewAnaliticSquareOne(Guid EncuestaId, List<ViewDataAnaliticDetail> DataBase, List<ViewDataAnaliticDetail> DatosBrutos)
        {
            List<GraphicBase> Result = new List<GraphicBase>();

            List<string> Paises = DataBase.Select(s => s.Pais).Distinct().ToList();
            List<string> Sucursales = DataBase.Select(s => s.Sucursal).Distinct().ToList();
            List<string> Gerencias = DataBase.Select(s => s.Gerencia).Distinct().ToList();
            List<string> Roles = DataBase.Select(s => s.Rol).Distinct().ToList();

            int Id = 0;
            foreach (string item in Paises)
            {
                string IdAsignaciones = string.Join("|", DatosBrutos.Where(e => e.Pais.Equals(item)).Select(s => s.Id.ToString()).ToList());
                List<SquareOne> TempCuadroUno = new List<SquareOne>();
                using (ClsAnalitics Analitica = new ClsAnalitics())
                {
                    TempCuadroUno = Analitica.AnaliticSquareOne(EncuestaId, IdAsignaciones);
                }
                if (TempCuadroUno.Count() > 0)
                {
                    GraphicBase CuadroUno = CuadroUnoGrafico(TempCuadroUno);
                    CuadroUno.Id = $"PaisUno_{Id}";
                    Id++;
                    CuadroUno.Titulo = $"General Pais {item}";
                    Result.Add(CuadroUno);
                }
            }

            foreach (string item in Sucursales)
            {
                string IdAsignaciones = string.Join("|", DatosBrutos.Where(e => e.Sucursal.Equals(item)).Select(s => s.Id.ToString()).ToList());
                List<SquareOne> TempCuadroUno = new List<SquareOne>();
                using (ClsAnalitics Analitica = new ClsAnalitics())
                {
                    TempCuadroUno = Analitica.AnaliticSquareOne(EncuestaId, IdAsignaciones);
                }
                if (TempCuadroUno.Count() > 0)
                {
                    GraphicBase CuadroUno = CuadroUnoGrafico(TempCuadroUno);
                    CuadroUno.Id = $"SucursalUno_{Id}";
                    Id++;
                    CuadroUno.Titulo = $"General Sucursal {item}";
                    Result.Add(CuadroUno);
                }
            }

            foreach (string item in Gerencias)
            {
                string IdAsignaciones = string.Join("|", DatosBrutos.Where(e => e.Gerencia.Equals(item)).Select(s => s.Id.ToString()).ToList());
                List<SquareOne> TempCuadroUno = new List<SquareOne>();
                using (ClsAnalitics Analitica = new ClsAnalitics())
                {
                    TempCuadroUno = Analitica.AnaliticSquareOne(EncuestaId, IdAsignaciones);
                }
                if (TempCuadroUno.Count() > 0)
                {
                    GraphicBase CuadroUno = CuadroUnoGrafico(TempCuadroUno);
                    CuadroUno.Id = $"GerenciaUno_{Id}";
                    Id++;
                    CuadroUno.Titulo = $"General Gerencia {item}";
                    Result.Add(CuadroUno);
                }
            }

            foreach (string item in Roles)
            {
                string IdAsignaciones = string.Join("|", DatosBrutos.Where(e => e.Rol.Equals(item)).Select(s => s.Id.ToString()).ToList());
                List<SquareOne> TempCuadroUno = new List<SquareOne>();
                using (ClsAnalitics Analitica = new ClsAnalitics())
                {
                    TempCuadroUno = Analitica.AnaliticSquareOne(EncuestaId, IdAsignaciones);
                }
                if (TempCuadroUno.Count() > 0)
                {
                    GraphicBase CuadroUno = CuadroUnoGrafico(TempCuadroUno);
                    CuadroUno.Id = $"RolUno_{Id}";
                    Id++;
                    CuadroUno.Titulo = $"General Rol {item}";
                    Result.Add(CuadroUno);
                }
            }

            return Result;
        }

        private GraphicBase CuadroUnoGrafico(List<SquareOne> Data)
        {
            GraphicBase Result = new GraphicBase
            {
                Series = new List<DataSeries>(),
                Categories = Data.Select(s => s.Grupo).Distinct().ToList(),
                Colors = new List<string>()
            };

            List<decimal> Minimo = new List<decimal>();
            Result.Colors.Add("#f55353");
            List<decimal> Moda = new List<decimal>();
            Result.Colors.Add("#d31818");
            List<decimal> Promedio = new List<decimal>();
            Result.Colors.Add("#42bdd9");
            List<decimal> Maximo = new List<decimal>();
            Result.Colors.Add("#143f6b");

            foreach (string item in Data.Select(s => s.Grupo).Distinct())
            {
                Promedio.AddRange(Data.Where(c => c.Grupo.Equals(item)).Select(s => s.Promedio).ToList());
                Maximo.AddRange(Data.Where(c => c.Grupo.Equals(item)).Select(s => Convert.ToDecimal(s.Maximo)).ToList());
                Minimo.AddRange(Data.Where(c => c.Grupo.Equals(item)).Select(s => Convert.ToDecimal(s.Minimo)).ToList());
                Moda.AddRange(Data.Where(c => c.Grupo.Equals(item)).Select(s => Convert.ToDecimal(s.Moda)).ToList());
            }

            Result.Series.Add(new DataSeries { data = Minimo, name = "Minimo" });
            Result.Series.Add(new DataSeries { data = Moda, name = "Moda" });
            Result.Series.Add(new DataSeries { data = Promedio, name = "Promedio" });
            Result.Series.Add(new DataSeries { data = Maximo, name = "Máximo" });

            return Result;
        }

        private GraphicTwo CuadroDosGrafico(Guid EncuestaId, List<ViewDataAnaliticDetail> DataBase)
        {
            GraphicTwo Result = new GraphicTwo();

            string DataJson = JsonConvert.SerializeObject(DataBase);
            List<SquareDetail> Data = new List<SquareDetail>();

            using (ClsAnalitics Analitica = new ClsAnalitics())
            {
                Data = Analitica.AnaliticSquareTwo(EncuestaId, DataJson);
            }

            List<string> Paises = Data.Select(d => d.Pais).Distinct().ToList();
            List<string> Sucursales = Data.Select(d => d.Sucursal).Distinct().ToList();
            List<string> Grupos = Data.Select(d => d.Grupo).Distinct().ToList();
            List<string> Colores = Data.Select(d => d.Color).Distinct().ToList();            

            Result.GenPais = new DataGeneral
            {
                Labels = new List<string>(),
                Data = new List<decimal>()
            };

            foreach (string Pais in Paises)
            {
                Result.GenPais.Labels.Add(Pais);
                List<Tuple<Guid, string>> AsignacionesPais = Data.GroupBy(d => d.IdAsignacion, d => d.Pais, (key, g) => new Tuple<Guid, string>(key, g.First())).ToList();                
                Result.GenPais.Data.Add(AsignacionesPais.Count(s => s.Item2.Equals(Pais)));
            }
            
            Result.GenSucursal = new DataGeneral
            {
                Labels = new List<string>(),
                Data = new List<decimal>()
            };

            foreach (string Sucursal in Sucursales)
            {
                Result.GenSucursal.Labels.Add(Sucursal);
                List<Tuple<Guid, string>> AsignacionesSucursal = Data.GroupBy(d => d.IdAsignacion, d => d.Sucursal, (key, g) => new Tuple<Guid, string>(key, g.First())).ToList();
                Result.GenSucursal.Data.Add(AsignacionesSucursal.Count(s => s.Item2.Equals(Sucursal)));
            }

            List<GraphicTwoBase> Base = new List<GraphicTwoBase>();

            for (int a = 0; a < Paises.Count(); a++)
            {
                int b = 0;
                foreach (var Sucursal in Data.Where(d => d.Pais.Equals(Paises[a])).Select(s => s.Sucursal).Distinct())
                {
                    GraphicTwoBase addItem = new GraphicTwoBase
                    {
                        Id = $"Pais_{a}_Sucursal_{b}_Dos",
                        Colors = Colores,
                        Labels = Grupos,
                        Titulo = $"Pais: {Paises[a]}, Sucursal: {Sucursal}",
                        Series = new List<decimal>()
                    };
                    foreach (var Grupo in Grupos)
                    {                        
                        IQueryable<decimal> DataAverage = Data.Where(d => d.Pais.Equals(Paises[a]) && d.Sucursal.Equals(Sucursal) && d.Grupo.Equals(Grupo)).Select(s => s.Promedio).AsQueryable();
                        addItem.Series.Add(Queryable.Average(DataAverage));
                    }
                    Base.Add(addItem);
                    b++;
                }
            }

            Result.GrafBase = Base;

            return Result;
        }

        [HttpPost]
        public PartialViewResult ListaDeEncuestados(string IdAliado, string IdEncuesta)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            ViewResponsePartial Result = new ViewResponsePartial
            {
                Idioma = Login.Idioma,
                Encuestados = new List<ViewResponsePollUser>()
            };

            if (!string.IsNullOrEmpty(IdAliado) && !IdAliado.Equals("0") && !string.IsNullOrEmpty(IdEncuesta) && !IdEncuesta.Equals("0"))
            {
                Guid AliadoId = new Guid(IdAliado);
                Guid EncuestaId = new Guid(IdEncuesta);
                Result.Encuestados = ListarEncuestados(AliadoId, EncuestaId);
            }

            return PartialView("_ListaEncuestados", Result);
        }

        private List<ViewResponsePollUser> ListarEncuestados(Guid AliadoId, Guid? EncuestaId)
        {
            List<ViewResponsePollUser> Encuestados = new List<ViewResponsePollUser>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Encuestados = (from s in db.Usuario
                               join a in db.UsuariosPorEncuenta on s.Id equals a.UsuarioId
                               where s.AliadoId == AliadoId
                               && ((EncuestaId == null) || (a.EncuestaId == EncuestaId))
                               && a.Resuelta == true
                               select new ViewResponsePollUser
                               {
                                   Id = a.Id,
                                   Correo = s.Correo,
                                   Nombres = s.Nombres,
                                   Apellidos = s.Apellidos,
                                   PaisId = s.PaisId,
                                   FechaRespuesta = (DateTime)a.FechaResuelta,
                               }).OrderBy(o => o.Correo).ToList();
            }

            foreach (ViewResponsePollUser item in Encuestados)
            {
                item.Sucursal = RamdomSucursal();
                item.Gerencia = RamdomGerencia();
                item.Rol = RamdomRol();
            }

            return Encuestados;
        }

        public int RandomNumber(int max)
        {
            return _random.Next(0, max);
        }

        private string RamdomSucursal()
        {
            List<string> Lista = new List<string>
            {
                "Colina",
                "Mayor",
                "Cedritos",
                "Americas",
                "Barbara",
                "Market",
                "Plaza",
                "Estación"
            };

            int Position = RandomNumber(Lista.Count);
            return Lista[Position];
        }

        private string RamdomGerencia()
        {
            List<string> Lista = new List<string>
            {
                "Economia",
                "Informatica",
                "Legal",
                "RH",
                "Gerencia",
                "Deportes",
            };

            int Position = RandomNumber(Lista.Count);
            return Lista[Position];
        }

        private string RamdomRol()
        {
            List<string> Lista = new List<string>
            {
                "Bachiller",
                "Tecnico",
                "Tecnologo",
                "Profesional",
                "Especialista",
                "Maestros",
                "Doctores",
                "Ceo"
            };

            int Position = RandomNumber(Lista.Count);
            return Lista[Position];
        }
    }
}