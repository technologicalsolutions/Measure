using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Encuesta;
using Measure.ViewModels.Error;
using Measure.ViewModels.Grupo;
using Measure.ViewModels.Pregunta;
using Measure.ViewModels.Respuesta;
using Measure.ViewModels.Usuario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class EncuestasController : Controller
    {
        [Route("Encuestas")]
        public ActionResult Index(Guid? Cliente)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ViewEncuesta Modelo = new ViewEncuesta
                {
                    ClienteId = Cliente,
                    RolId = Login.RolId,
                    TipoReporteGeneral = new List<SelectListItem> { new SelectListItem { Text = "Seleccione...", Value = "0" } },
                };

                List<ViewPoll> Lista = new List<ViewPoll>();
                List<Reporte> Reportes = new List<Reporte>();
                if (Cliente == Guid.Empty || Cliente == null)
                {
                    Lista = db.Encuesta.Select(s => new ViewPoll
                    {
                        ActualizaUsuario = s.ActualizaUsuario,
                        ClienteId = s.ClienteId,
                        Estado = s.Estado,
                        FechaCreacion = s.FechaCreacion,
                        id = s.id,
                        Nombre = s.Nombre,
                        Proposito = s.Proposito
                    }).ToList();

                    Reportes = db.Reporte.Where(r => r.Estado).ToList();

                    Modelo.Modelo = new ViewPoll();
                    Modelo.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                }
                else
                {
                    Modelo.Cliente = db.Usuario.FirstOrDefault(u => u.Id == Cliente);

                    if (Modelo.Cliente.RolId == (int)Enums.UserRol.Aliado)
                    {
                        Lista = (from b in db.UsuariosPorEncuenta
                                 join s in db.Encuesta on b.EncuestaId equals s.id
                                 where b.UsuarioId == Cliente
                                 select new ViewPoll
                                 {
                                     ActualizaUsuario = s.ActualizaUsuario,
                                     ClienteId = s.ClienteId,
                                     Estado = s.Estado,
                                     FechaCreacion = s.FechaCreacion,
                                     id = s.id,
                                     Nombre = s.Nombre,
                                     Proposito = s.Proposito
                                 }).ToList();

                    }
                    else if (Modelo.Cliente.RolId == (int)Enums.UserRol.Cliente)
                    {
                        Lista = db.Encuesta.Where(e => e.ClienteId == Cliente).
                                Select(s => new ViewPoll
                                {
                                    ActualizaUsuario = s.ActualizaUsuario,
                                    ClienteId = s.ClienteId,
                                    Estado = s.Estado,
                                    FechaCreacion = s.FechaCreacion,
                                    id = s.id,
                                    Nombre = s.Nombre,
                                    Proposito = s.Proposito
                                }).ToList();
                    }

                    Reportes = db.Reporte.Where(r => r.ClienteId == Cliente && r.Estado).ToList();
                }

                Modelo.TipoReporteGeneral = Reportes.Select(r => new SelectListItem
                {
                    Text = Login.Idioma == (int)Enums.Idiomas.es_ES ? r.es_ES : Login.Idioma == (int)Enums.Idiomas.en_US ? r.en_US : r.pt_BR,
                    Value = r.Id.ToString(),
                }).ToList();

                Modelo.Lista = Lista;
                return View(Modelo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EncuestaAcciones")]
        public ActionResult Acciones(ViewEncuesta Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Encuesta encuesta = ResultPoll(Data.Modelo);
                    switch (Data.Accion)
                    {
                        case Enums.DbAcciones.Crea:
                            encuesta.FechaCreacion = DateTime.Now;
                            db.Encuesta.Add(encuesta);
                            db.SaveChanges();

                            if (encuesta.ActualizaUsuario)
                            {
                                ContenidoPorEncuesta update = new ContenidoPorEncuesta
                                {
                                    ComponenteId = Guid.Empty,
                                    EncuestaId = encuesta.id,
                                    Estado = true,
                                    Id = Guid.NewGuid(),
                                    Orden = 0,
                                    TipoComponente = 0
                                };
                                db.ContenidoPorEncuesta.Add(update);
                                db.SaveChanges();
                            }
                            break;
                        case Enums.DbAcciones.Actualiza:
                            Encuesta _OldEncuesta = db.Encuesta.Where(u => u.id == encuesta.id).FirstOrDefault();
                            Encuesta OldEncuesta = ComparePoll(_OldEncuesta, encuesta);
                            db.Entry(OldEncuesta).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Result = ClaseUtil.CatchException(e);
                }
            }

            return RedirectToAction("Index", Data.Modelo.ClienteId);
        }

        [HttpPost]
        public PartialViewResult Detail(ViewEncuesta Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Guid Id = (Guid)Data.Modelo.id;
                Data.Modelo = ResultViewPoll(db.Encuesta.Where(u => u.id == Id).FirstOrDefault());
            }
            return PartialView("_Details", Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Encuesta encuesta = db.Encuesta.Where(u => u.id == id).First();
                db.Encuesta.Remove(encuesta);
                db.SaveChanges();

                return RedirectToRoute("Encuestas", new { Cliente = Login.ClienteId });
            }
        }

        [HttpPost]
        public ActionResult ResponderEncuesta(Guid Id)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewAnswerSurvey Modelo = new ViewAnswerSurvey
            {
                Lista = new List<ViewContentListAnswer>(),
            };
            List<ContenidoPorEncuesta> TempContenido = new List<ContenidoPorEncuesta>();
            Encuesta encuesta = new Encuesta();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.Cliente = db.Usuario.Find(Login.ClienteId);
                Modelo.Asignacion = db.UsuariosPorEncuenta.Find(Id);
                encuesta = db.Encuesta.Find(Modelo.Asignacion.EncuestaId);
                TempContenido = db.ContenidoPorEncuesta.Where(c => c.EncuestaId == Modelo.Asignacion.EncuestaId).OrderBy(o => o.Orden).ToList();
            }

            List<ContenidoPorEncuesta> Contenido = new List<ContenidoPorEncuesta>();
            if (encuesta.ActualizaUsuario)
            {
                ContenidoPorEncuesta UpdateUser = TempContenido.FirstOrDefault(t => t.ComponenteId == Guid.Empty);
                TempContenido.Remove(UpdateUser);
                foreach (var item in TempContenido)
                {
                    if (item.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta
                            && Contenido.Count(c => c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta) == 0)
                    {
                        Contenido.Add(UpdateUser);
                    }
                    Contenido.Add(item);
                }
            }
            else
            {
                Contenido = TempContenido;
            }

            foreach (ContenidoPorEncuesta item in Contenido)
            {
                ViewContentListAnswer Item = new ViewContentListAnswer();

                switch ((Enums.TipoComponente)item.TipoComponente)
                {
                    case Enums.TipoComponente.ActualizarUsuario:
                        Item.ActulizarUsuario = new ViewUpdateUser
                        {
                            Idioma = Login.Idioma,
                            DataPage = ActualizarUsuario(Login)
                        };
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Item.ActulizarUsuario.Group = db.Grupo.Find(item.ComponenteId);
                        }       
                        
                        Modelo.Lista.Add(Item);
                        break;
                    case Enums.TipoComponente.Inicio:
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Item.Inicio = db.Contenido.FirstOrDefault(c => c.Idioma == Login.Idioma && c.Id == item.ComponenteId);
                        }
                        if (Item.Inicio != null)
                        {
                            Modelo.Lista.Add(Item);
                        }
                        break;
                    case Enums.TipoComponente.CategoriaEncuesta:
                        ViewLoadGroupPoll AddGroup = new ViewLoadGroupPoll();
                        List<Pregunta> Preguntas = new List<Pregunta>();

                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            AddGroup.Group = db.Grupo.Find(item.ComponenteId);
                            List<Guid> IdPreguntas = db.PreguntasPorGrupo.Where(p => p.GrupoId == item.ComponenteId).OrderBy(o => o.Orden).Select(s => s.PreguntaId).ToList();
                            Preguntas = db.Pregunta.Where(p => IdPreguntas.Contains(p.Id) && p.Idioma == Login.Idioma && p.Estado).OrderBy(o => o.Texto).ToList();
                        }
                        AddGroup.Preguntas = new List<ViewLoadQuestion>();
                        foreach (Pregunta ItemPr in Preguntas)
                        {
                            ViewLoadQuestion AddItemPr = new ViewLoadQuestion
                            {
                                Question = ItemPr,
                            };

                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                switch ((Enums.TipoDeControl)ItemPr.TipoControl)
                                {
                                    case Enums.TipoDeControl.ControlAbierto:
                                        AddItemPr.ControlesAbiertos = db.ControlAbierto.Find(ItemPr.ControlId);
                                        break;
                                    case Enums.TipoDeControl.ControlBooleano:
                                        AddItemPr.ControlesBooleanos = db.ControlBooleano.Find(ItemPr.ControlId);
                                        break;
                                    case Enums.TipoDeControl.ControlCerrado:
                                        AddItemPr.ControlesCerrados = db.ControlCerrado.Find(ItemPr.ControlId);
                                        break;
                                    case Enums.TipoDeControl.ControlMatriz:
                                        AddItemPr.ControlesMatrices = new ViewAnswerControlMatriz
                                        {
                                            ControlM = db.ControlMatriz.Find(ItemPr.ControlId),
                                            ControlMColunnas = db.ControlMatrizColumna.Where(c => c.MatrizId == ItemPr.ControlId).OrderBy(o => o.Orden).ToList(),
                                            ControlMFilas = db.ControlMatrizFila.Where(c => c.MatrizId == ItemPr.ControlId).OrderBy(o => o.Orden).ToList()
                                        };
                                        break;
                                }
                            }
                            AddGroup.Preguntas.Add(AddItemPr);
                        }

                        if (AddGroup.Preguntas.Count() > 0)
                        {
                            Item.Categoria = AddGroup;
                            Modelo.Lista.Add(Item);
                        }
                        break;
                    case Enums.TipoComponente.Fin:
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Item.Fin = db.Contenido.FirstOrDefault(c => c.Idioma == Login.Idioma && c.Id == item.ComponenteId);
                        }
                        if (Item.Fin != null)
                        {
                            Modelo.Lista.Add(Item);
                        }
                        break;
                    case Enums.TipoComponente.Grabar:
                        using (ModeloEncuesta db = new ModeloEncuesta())
                        {
                            Item.Grabar = db.Contenido.FirstOrDefault(c => c.Idioma == Login.Idioma && c.Id == item.ComponenteId);
                        }
                        if (Item.Grabar != null)
                        {
                            Modelo.Lista.Add(Item);
                        }
                        break;
                }
            }
            return View(Modelo);
        }

        public ActionResult MisEncuestas(Guid? Id)
        {
            if (HttpContext.Session["login"] == null || Id == null)
            {
                return RedirectToAction("index", "Login");
            }

            using (ClsProcedures clsProcedures = new ClsProcedures())
            {
                ViewPollUser Modelo = new ViewPollUser
                {
                    Lista = clsProcedures.MisAsignaciones((Guid)Id, true)
                };
                return View(Modelo);
            }
        }

        [HttpPost]
        [Route("GuardarEncuesta")]
        public ActionResult GuardarEncuesta(string Json)
        {
            ViewCaptureResponse respuestas = JsonConvert.DeserializeObject<ViewCaptureResponse>(Json);
            Guid UserId = new Guid(respuestas.IdUsuario);

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (respuestas.Update != null)
                {
                    Usuario Update = db.Usuario.Find(UserId);

                    Update.Nombres = respuestas.Update.Nombres;
                    Update.Apellidos = respuestas.Update.Apellidos;
                    Update.Telefono = respuestas.Update.Telefono;
                    Update.Titulo = respuestas.Update.Titulo;
                    Update.CorreoTrabajo = respuestas.Update.CorreoTrabajo;
                    Update.Empresa = respuestas.Update.Empresa;
                    Update.CEmpleadosId = respuestas.Update.CEmpleadosId;
                    Update.IndustriaId = respuestas.Update.IndustriaId;
                    Update.Subsidiario = respuestas.Update.Subsidiario;
                    Update.PaisId = respuestas.Update.PaisId;
                    Update.DireccionTrabajo = respuestas.Update.DireccionTrabajo;
                    Update.Provincia = respuestas.Update.Provincia;

                    db.Entry(Update).State = EntityState.Modified;
                    db.SaveChanges();
                }

                foreach (ViewAnswer item in respuestas.Respuestas)
                {
                    if (item.Respuesta != null)
                    {
                        if (item.Respuesta.Count() > 0)
                        {                            
                            Pregunta _pregunta = db.Pregunta.Where(p => p.ControlId == new Guid(item.Id) && p.Estado).FirstOrDefault();
                            if (_pregunta != null)
                            {
                                string Result = string.Join("|", item.Respuesta);
                                Respuesta subitem = new Respuesta
                                {
                                    Id = Guid.NewGuid(),
                                    IdAsignacion = new Guid(respuestas.IdAsignacion),
                                    PreguntaId = _pregunta.Id,
                                    UsuarioId = UserId,
                                    Resultado = string.IsNullOrEmpty(Result) ? " " : Result,
                                    TipoControl = item.TipoControl,
                                    FechaRespuesta = DateTime.Now
                                };
                                db.Respuesta.Add(subitem);
                            }
                        }
                    }
                }
                db.SaveChanges();

                UsuariosPorEncuenta usuariosPor = db.UsuariosPorEncuenta.Find(new Guid(respuestas.IdAsignacion));
                usuariosPor.Resuelta = true;
                usuariosPor.FechaResuelta = DateTime.Now;

                db.Entry(usuariosPor).State = EntityState.Modified;
                db.SaveChanges();
            }

            using (ClsProcedures clsProcedures = new ClsProcedures())
            {
                ViewPoll _Encuesta = clsProcedures.MisAsignaciones(UserId, false).FirstOrDefault();

                return RedirectToRoute("Email", new ViewReport { Id = new Guid(respuestas.IdAsignacion), Report = (_Encuesta == null) });
            }
        }

        public ViewPoll ResultViewPoll(Encuesta Data)
        {
            ViewPoll Result = new ViewPoll
            {
                ActualizaUsuario = Data.ActualizaUsuario,
                ClienteId = Data.ClienteId,
                Estado = Data.Estado,
                FechaCreacion = Data.FechaCreacion,
                id = Data.id,
                Nombre = Data.Nombre,
                Proposito = Data.Proposito,
            };

            return Result;
        }

        public Encuesta ResultPoll(ViewPoll Data)
        {
            Encuesta Result = new Encuesta
            {
                ActualizaUsuario = Data.ActualizaUsuario,
                ClienteId = (Guid)Data.ClienteId,
                Estado = Data.Estado,
                FechaCreacion = Data.FechaCreacion,
                id = Data.id == null ? Guid.NewGuid() : (Guid)Data.id,
                Nombre = Data.Nombre,
                Proposito = Data.Proposito
            };
            return Result;
        }

        [HttpGet]
        public JsonResult ListaEncuestas(string ClienteId)
        {
            List<SelectListItem> Lista = new List<SelectListItem>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Guid Cliente = new Guid(ClienteId);
                Lista = db.Encuesta.Where(e => e.ClienteId == Cliente).
                        Select(s => new SelectListItem
                        {
                            Text = s.Nombre,
                            Value = s.id.ToString()
                        }).ToList();
            }
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private Encuesta ComparePoll(Encuesta OldEncuesta, Encuesta NewEncuesta)
        {
            OldEncuesta.Estado = OldEncuesta.Estado != NewEncuesta.Estado ? NewEncuesta.Estado : OldEncuesta.Estado;
            OldEncuesta.Nombre = OldEncuesta.Nombre != NewEncuesta.Nombre ? NewEncuesta.Nombre : OldEncuesta.Nombre;
            OldEncuesta.Proposito = OldEncuesta.Proposito != NewEncuesta.Proposito ? NewEncuesta.Proposito : OldEncuesta.Proposito;
            OldEncuesta.ClienteId = OldEncuesta.ClienteId != NewEncuesta.ClienteId ? NewEncuesta.ClienteId : OldEncuesta.ClienteId;

            return OldEncuesta;
        }

        private List<ViewUserProperties> ActualizarUsuario(ViewLogin Login)
        {
            List<ViewUserProperties> Result = new List<ViewUserProperties>();

            List<UsuarioLabel> Data = DatosPagina();
            foreach (UsuarioLabel item in Data)
            {
                PropertyInfo Property = Login.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(item.Campo));

                Result.Add(new ViewUserProperties
                {
                    Confirm = item.Confirmacion,
                    Compare = item.Comparar,
                    Label = Login.Idioma == (int)Enums.Idiomas.es_ES ? item.es_ES : Login.Idioma == (int)Enums.Idiomas.en_US ? item.en_US : item.pt_BR,
                    Name = item.Campo,
                    Value = Property.GetValue(Login) != null ? Property.GetValue(Login, null).ToString() : string.Empty,
                });

                if (item.Comparar)
                {
                    Result.Add(new ViewUserProperties
                    {
                        Confirm = item.Confirmacion,
                        Compare = item.Comparar,
                        Label = string.Format("{0} {1}", Recursos.Recurso.Confirmar, (Login.Idioma == (int)Enums.Idiomas.es_ES ? item.es_ES : Login.Idioma == (int)Enums.Idiomas.en_US ? item.en_US : item.pt_BR)),
                        Name = string.Concat("Comparar", item.Campo),
                        Value = Property.GetValue(Login) != null ? Property.GetValue(Login, null).ToString() : string.Empty,
                    });
                }
            }

            return Result;
        }

        private List<UsuarioLabel> DatosPagina()
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                return db.UsuarioLabel.OrderBy(o => o.Orden).ToList();
            }
        }
    }
}