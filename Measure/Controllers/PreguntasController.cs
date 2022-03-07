using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Error;
using Measure.ViewModels.Pregunta;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class PreguntasController : Controller
    {
        [Route("Preguntas")]
        public ActionResult Index(Guid? Cliente)
        {
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            if (Login == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewPreguntasFilter Modelo = new ViewPreguntasFilter
            {
                Clientes = new List<SelectListItem>(),
                TipoControl = new List<SelectListItem> {
                    new SelectListItem { Text = Recursos.Recurso.ControlTodo },
                    new SelectListItem { Value = "0", Text = Recursos.Recurso.ControlAbierto },
                    new SelectListItem { Value = "1", Text = Recursos.Recurso.ControlBooleano },
                    new SelectListItem { Value = "2", Text = Recursos.Recurso.ControlCerrado },
                    new SelectListItem { Value = "3", Text = Recursos.Recurso.ControlMatriz }
                }
            };
            Enums.Idiomas BasePlataforma = (Enums.Idiomas)HttpContext.Session["BaseIdioma"];

            List<MaestrasDetalle> Lenguaje = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Lenguaje = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.ToList();
            }

            switch (BasePlataforma)
            {
                case Enums.Idiomas.es_ES:
                    Modelo.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.es_ES, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
                case Enums.Idiomas.en_US:
                    Modelo.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.en_US, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
                case Enums.Idiomas.pt_BR:
                    Modelo.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.pt_BR, Value = s.Valor, Selected = (s.Id == (int)BasePlataforma) }).ToList();
                    break;
            }

            if (Cliente == null || Cliente == Guid.Empty)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Modelo.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList());
                }
                if (Modelo.Clientes.Count() == 0)
                {
                    Modelo.Clientes.Add(new SelectListItem { Value = "0", Text = "No hay Clientes" });
                }
            }
            else
            {
                Modelo.ClienteId = (Guid)Cliente;
            }

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.ListaPreguntas = (from A in db.Pregunta
                                         where (Modelo.ClienteId == null || A.ClienteId == Modelo.ClienteId)
                                         select new ViewPregunta
                                         {
                                             ControlId = A.ControlId,
                                             Estado = A.Estado,
                                             EstadoName = (A.Estado ? Recursos.Recurso.EstadoActivo : Recursos.Recurso.EstadoInactivo),
                                             Id = A.Id,
                                             Idioma = A.Idioma,
                                             IdiomaName = (A.Idioma == 1 ? Recursos.Recurso.IdiomaEspanol : A.Idioma == 2 ? Recursos.Recurso.IdiomaIngles : Recursos.Recurso.IdiomaPortugues),
                                             Texto = A.Texto,
                                             TipoControl = A.TipoControl,
                                             ClienteId = A.ClienteId,
                                             Confirmacion = A.Confirmacion
                                         }).ToList();
            }

            return View(Modelo);
        }

        [Route("Pregunta")]
        public ActionResult Pregunta(ViewQuestion Data)
        {
            ViewLogin login = HttpContext.Session["login"] as ViewLogin;

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            string view = "Create";
            List<SelectListItem> Maestras = new List<SelectListItem>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Maestras = db.Maestras.Where(m => m.ClienteId == Data.ClienteId && m.ClienteId == Guid.Empty).
                           Select(s => new SelectListItem
                           {
                               Value = s.Id.ToString(),
                               Text = login.Idioma == (int)Enums.Idiomas.es_ES ? s.es_ES : login.Idioma == (int)Enums.Idiomas.en_US ? s.en_US : s.pt_BR
                           }).ToList();
            }

            switch (Data.Accion)
            {
                case Enums.DbAcciones.Crea:
                    Data.Maestras = Maestras;
                    ViewBag.Title = "Crear";
                    break;
                case Enums.DbAcciones.Actualiza:
                    ViewBag.Title = "Editar";
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Data = ResultViewQuestion(db.Pregunta.Find((Guid)Data.Id));

                        switch ((Enums.TipoDeControl)Data.TipoControl)
                        {
                            case Enums.TipoDeControl.ControlAbierto:
                                Data.ControlA = db.ControlAbierto.Find(Data.ControlId);
                                view = "EditOpen";
                                break;
                            case Enums.TipoDeControl.ControlBooleano:
                                Data.ControlB = db.ControlBooleano.Find(Data.ControlId);
                                view = "EditBool";
                                break;
                            case Enums.TipoDeControl.ControlCerrado:
                                Data.ControlC = db.ControlCerrado.Find(Data.ControlId);
                                Data.Maestras = Maestras;
                                view = "EditClosed";
                                break;
                            case Enums.TipoDeControl.ControlMatriz:
                                Data.ControlM = db.ControlMatriz.Find(Data.ControlId);
                                Data.ControlMColunnas = db.ControlMatrizColumna.Where(c => c.MatrizId == Data.ControlId).OrderBy(o => o.Orden).ToList();
                                Data.ControlMFilas = db.ControlMatrizFila.Where(c => c.MatrizId == Data.ControlId).OrderBy(o => o.Orden).ToList();
                                view = "EditMatrix";
                                break;
                        }
                        break;
                    }
            }
            return View(view, Data);
        }

        public ViewQuestion ResultViewQuestion(Pregunta Data)
        {
            ViewQuestion Result = new ViewQuestion
            {
                ClienteId = Data.ClienteId,
                Confirmacion = Data.Confirmacion,
                ControlId = Data.ControlId,
                Estado = Data.Estado,
                Id = Data.Id,
                Idioma = Data.Idioma,
                IdiomaName = (Data.Idioma == (int)Enums.Idiomas.es_ES ? Recursos.Recurso.IdiomaEspanol : Data.Idioma == (int)Enums.Idiomas.en_US ? Recursos.Recurso.IdiomaIngles : Recursos.Recurso.IdiomaPortugues),
                Texto = Data.Texto,
                TipoControl = Data.TipoControl,
            };
            return Result;
        }

        [HttpPost]
        public JsonResult ActualizarControlAbierto(ViewControlAbierto Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {

                    ControlAbierto Control = new ControlAbierto
                    {
                        Id = Data.ControlId,
                        Antes = Data.Antes,
                        DescripcionPos = Data.DescripcionPos,
                        DescripcionPre = Data.DescripcionPre,
                        MultiCampo = Data.MultiCampo,
                        Multilinea = Data.Multilinea,
                        TipoAbierta = Data.TipoAbierta,
                        TipoDato = Data.TipoDato
                    };

                    db.Entry(Control).State = EntityState.Modified;
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Data.PreguntaId,
                        ClienteId = Data.ClienteId,
                        Confirmacion = Data.Confirmacion,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlAbierto,
                        Texto = Data.TextoPregunta
                    };

                    db.Entry(_Pregunta).State = EntityState.Modified;
                    db.SaveChanges();


                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult GuardarControlAbierto(ViewControlAbierto Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlAbierto Control = new ControlAbierto
                    {
                        Id = Guid.NewGuid(),
                        Antes = Data.Antes,
                        DescripcionPos = Data.DescripcionPos,
                        DescripcionPre = Data.DescripcionPre,
                        MultiCampo = Data.MultiCampo,
                        Multilinea = Data.Multilinea,
                        TipoAbierta = Data.TipoAbierta,
                        TipoDato = Data.TipoDato
                    };

                    db.ControlAbierto.Add(Control);
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Guid.NewGuid(),
                        ClienteId = Data.ClienteId,
                        Confirmacion = Data.Confirmacion,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlAbierto,
                        Texto = Data.TextoPregunta
                    };

                    db.Pregunta.Add(_Pregunta);
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult ActualizarControlBoolean(ViewControlBool Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlBooleano Control = new ControlBooleano
                    {
                        Id = Data.ControlId,
                        RespuestaCorrecta = Data.RespuestaCorrecta,
                        TipoValor = Data.TipoValor
                    };

                    db.Entry(Control).State = EntityState.Modified;
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Data.PreguntaId,
                        ClienteId = Data.ClienteId,
                        ControlId = Data.ControlId,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlBooleano,
                        Texto = Data.TextoPregunta
                    };

                    db.Entry(_Pregunta).State = EntityState.Modified;
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult GuardarControlBoolean(ViewControlBool Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlBooleano Control = new ControlBooleano
                    {
                        Id = Guid.NewGuid(),
                        RespuestaCorrecta = Data.RespuestaCorrecta,
                        TipoValor = Data.TipoValor
                    };

                    db.ControlBooleano.Add(Control);
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Guid.NewGuid(),
                        ClienteId = Data.ClienteId,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlBooleano,
                        Texto = Data.TextoPregunta
                    };

                    db.Pregunta.Add(_Pregunta);
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult ActualizarControlMatriz(ViewControlMatriz Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlMatriz Control = db.ControlMatriz.Find(Data.ControlId);
                    Control.Maximo = Data.Max;
                    Control.Minimo = Data.Min;
                    Control.Step = (int)Data.Step;

                    db.Entry(Control).State = EntityState.Modified;
                    db.SaveChanges();

                    List<ControlMatrizColumna> OldCol = db.ControlMatrizColumna.Where(c => c.MatrizId == Data.ControlId).ToList();
                    db.ControlMatrizColumna.RemoveRange(OldCol);
                    db.SaveChanges();

                    List<ControlMatrizColumna> Columnas = new List<ControlMatrizColumna>();
                    List<string> Columns = Data.TextoColumnas.Split('|').ToList();
                    List<string> Steps = Data.PasosPorColumna.Split('|').ToList();
                    for (int a = 0; a < Columns.Count(); a++)
                    {
                        string col = Columns[a];
                        int stp = Convert.ToInt32(Steps[a]);
                        Columnas.Add(new ControlMatrizColumna
                        {
                            Id = Guid.NewGuid(),
                            PasosPorColumna = stp,
                            MatrizId = Control.Id,
                            Texto = col,
                            Orden = (a + 1)
                        });
                    }

                    db.ControlMatrizColumna.AddRange(Columnas);
                    db.SaveChanges();

                    List<ControlMatrizFila> OldRow = db.ControlMatrizFila.Where(c => c.MatrizId == Data.ControlId).ToList();
                    db.ControlMatrizFila.RemoveRange(OldRow);
                    db.SaveChanges();

                    List<ControlMatrizFila> Filas = new List<ControlMatrizFila>();
                    List<string> Rows = Data.TextoFilas.Split('|').ToList();
                    for (int a = 0; a < Rows.Count(); a++)
                    {            
                        string item = Rows[a];
                        Filas.Add(new ControlMatrizFila
                        {
                            Id = Guid.NewGuid(),
                            MatrizId = Control.Id,
                            Texto = item,
                            Orden = (a + 1)
                        });
                    }

                    db.ControlMatrizFila.AddRange(Filas);
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Data.PreguntaId,
                        ClienteId = Data.ClienteId,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlMatriz,
                        Texto = Data.TextoPregunta
                    };

                    db.Entry(_Pregunta).State = EntityState.Modified;
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult GuardarControlMatriz(ViewControlMatriz Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlMatriz Control = new ControlMatriz
                    {
                        Id = Guid.NewGuid(),
                        Maximo = Data.Max,
                        Minimo = Data.Min,
                        Step = (int)Data.Step,
                        TipoControl = (int)Data.Radio,
                    };

                    db.ControlMatriz.Add(Control);
                    db.SaveChanges();

                    List<ControlMatrizColumna> Columnas = new List<ControlMatrizColumna>();
                    List<string> Columns = Data.TextoColumnas.Split('|').ToList();
                    List<string> Steps = Data.PasosPorColumna.Split('|').ToList();
                    for (int a = 0; a < Columns.Count(); a++)
                    {
                        string col = Columns[a];
                        int stp = Convert.ToInt32(Steps[a]);
                        Columnas.Add(new ControlMatrizColumna
                        {
                            Id = Guid.NewGuid(),
                            PasosPorColumna = stp,
                            MatrizId = Control.Id,
                            Texto = col,
                            Orden = (a + 1)
                        });
                    }

                    db.ControlMatrizColumna.AddRange(Columnas);
                    db.SaveChanges();

                    List<ControlMatrizFila> Filas = new List<ControlMatrizFila>();                    
                    List<string> Rows = Data.TextoFilas.Split('|').ToList();
                    for (int a = 0; a < Rows.Count(); a++)
                    {
                        string item = Rows[a];
                        Filas.Add(new ControlMatrizFila
                        {
                            Id = Guid.NewGuid(),
                            MatrizId = Control.Id,
                            Texto = item,
                            Orden = (a + 1)
                        });
                    }

                    db.ControlMatrizFila.AddRange(Filas);
                    db.SaveChanges();

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Guid.NewGuid(),
                        ControlId = Control.Id,
                        ClienteId = Data.ClienteId,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlMatriz,
                        Texto = Data.TextoPregunta
                    };

                    db.Pregunta.Add(_Pregunta);
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult ActualizarControlCerrado(ViewControlCerrado Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlCerrado Control = new ControlCerrado
                    {
                        Id = Data.ControlId,
                        PadreId = Data.PadreId,
                        Radio = Data.Radio,
                        RespuestaCorrecta = Data.RespuestaCorrecta,
                        Textos = Data.Textos,
                        TieneControl = Data.TieneControl,
                        TipoCerrada = Data.TipoCerrada,
                        TipoDato = Data.TipoDato,
                        ValorControl = Data.ValorControl,
                        Valores = Data.Valores
                    };

                    if (Data.TipoCerrada == (int)Enums.ControlCerrado.Cc_Desplegable)
                    {
                        int MaestraId = Convert.ToInt32(Data.Valores);
                        List<MaestrasDetalle> Detalles = db.MaestrasDetalle.Where(d => d.MaestraId == MaestraId).ToList();
                        Control.Textos = string.Join("|", Detalles.Select(s => string.Format("{0};{1};{2}", s.es_ES, s.en_US, s.pt_BR)).ToList());
                        Control.Valores = string.Join("|", Detalles.Select(s => s.Valor).ToList());
                    }

                    db.Entry(Control).State = EntityState.Modified;
                    db.SaveChanges();

                    if (Control.PadreId != null)
                    {
                        ControlCerrado ControlPadre = db.ControlCerrado.Find(Control.PadreId);
                        ControlPadre.TieneControl = true;

                        db.Entry(ControlPadre).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Data.PreguntaId,
                        ClienteId = Data.ClienteId,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlCerrado,
                        Texto = Data.TextoPregunta
                    };

                    db.Entry(_Pregunta).State = EntityState.Modified;
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public JsonResult GuardarControlCerrado(ViewControlCerrado Data)
        {
            try
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    ControlCerrado Control = new ControlCerrado
                    {
                        Id = Guid.NewGuid(),
                        PadreId = Data.PadreId,
                        Radio = Data.Radio,
                        RespuestaCorrecta = Data.RespuestaCorrecta,
                        TieneControl = Data.TieneControl,
                        TipoCerrada = Data.TipoCerrada,
                        TipoDato = Data.TipoDato,
                        ValorControl = Data.ValorControl,
                        Textos = Data.Textos,
                        Valores = Data.Valores,
                    };

                    if (Data.TipoCerrada == (int)Enums.ControlCerrado.Cc_Desplegable)
                    {
                        int MaestraId = Convert.ToInt32(Data.Valores);
                        List<MaestrasDetalle> Detalles = db.MaestrasDetalle.Where(d => d.MaestraId == MaestraId).ToList();
                        Control.Textos = string.Join("|", Detalles.Select(s => string.Format("{0};{1};{2}", s.es_ES, s.en_US, s.pt_BR)).ToList());
                        Control.Valores = string.Join("|", Detalles.Select(s => s.Valor).ToList());
                    }

                    db.ControlCerrado.Add(Control);
                    db.SaveChanges();

                    if (Control.PadreId != null)
                    {
                        ControlCerrado ControlPadre = db.ControlCerrado.Find(Control.PadreId);
                        ControlPadre.TieneControl = true;

                        db.Entry(ControlPadre).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    Pregunta _Pregunta = new Pregunta
                    {
                        Id = Guid.NewGuid(),
                        ClienteId = Data.ClienteId,
                        ControlId = Control.Id,
                        Estado = Data.EstadoPregunta,
                        Idioma = Data.Idioma,
                        TipoControl = (int)Enums.TipoDeControl.ControlCerrado,
                        Texto = Data.TextoPregunta
                    };

                    db.Pregunta.Add(_Pregunta);
                    db.SaveChanges();

                    Guid? Cliente = null;
                    if (HttpContext.Session["login"] != null)
                    {
                        ViewLogin Login = HttpContext.Session["login"] as ViewLogin;
                        Cliente = Login.RolId == (int)Enums.UserRol.Administrador ? null : Login.RolId == (int)Enums.UserRol.Cliente ? Login.Id : Login.ClienteId;
                    }

                    return new JsonResult { Data = Cliente.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            catch (DbEntityValidationException e)
            {
                using (ClsUtilities ClaseUtil = new ClsUtilities())
                {
                    ViewCatchError Error = ClaseUtil.CatchException(e);
                    return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        [HttpPost]
        public PartialViewResult Detail(ViewQuestion Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ViewDetailQuestion Result = ResultViewDetailQuestion(db.Pregunta.Find((Guid)Data.Id));

                return PartialView("_Details", Result);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Pregunta _Pregunta = db.Pregunta.Find(id);

                switch ((Enums.TipoDeControl)_Pregunta.TipoControl)
                {
                    case Enums.TipoDeControl.ControlAbierto:
                        ControlAbierto CtrlA = db.ControlAbierto.Find(_Pregunta.ControlId);
                        db.ControlAbierto.Remove(CtrlA);
                        break;
                    case Enums.TipoDeControl.ControlBooleano:
                        ControlBooleano CtrlB = db.ControlBooleano.Find(_Pregunta.ControlId);
                        db.ControlBooleano.Remove(CtrlB);
                        break;
                    case Enums.TipoDeControl.ControlCerrado:
                        ControlCerrado CtrlC = db.ControlCerrado.Find(_Pregunta.ControlId);
                        db.ControlCerrado.Remove(CtrlC);
                        break;
                    case Enums.TipoDeControl.ControlMatriz:
                        ControlMatriz CtrlM = db.ControlMatriz.Find(_Pregunta.ControlId);
                        db.ControlMatriz.Remove(CtrlM);
                        break;
                }
                db.SaveChanges();

                db.Pregunta.Remove(_Pregunta);
                db.SaveChanges();
                return RedirectToRoute("Preguntas", id);
            }
        }

        public ViewDetailQuestion ResultViewDetailQuestion(Pregunta Data)
        {
            ViewDetailQuestion Result = new ViewDetailQuestion
            {
                Estado = Data.Estado ? Recursos.Recurso.EstadoActivo : Recursos.Recurso.EstadoInactivo,
                Texto = Data.Texto,
            };

            switch ((Enums.TipoDeControl)Data.TipoControl)
            {
                case Enums.TipoDeControl.ControlAbierto:
                    Result.TipoControl = Recursos.Recurso.ControlAbierto;
                    break;
                case Enums.TipoDeControl.ControlBooleano:
                    Result.TipoControl = Recursos.Recurso.ControlBooleano;
                    break;
                case Enums.TipoDeControl.ControlCerrado:
                    Result.TipoControl = Recursos.Recurso.ControlCerrado;
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        ControlCerrado CtrlC = db.ControlCerrado.Find(Data.ControlId);
                        if (CtrlC.PadreId != null)
                        {
                            Pregunta _Pregunta = db.Pregunta.Where(p => p.ControlId == CtrlC.PadreId).FirstOrDefault();
                            Result.Padre = _Pregunta.Texto;
                            Result.ValorPadre = CtrlC.RespuestaCorrecta;
                        }
                    }
                    break;
                case Enums.TipoDeControl.ControlMatriz:
                    Result.TipoControl = Recursos.Recurso.ControlMatriz;
                    break;
            }

            string Idioma = string.Empty;
            switch ((Enums.Idiomas)Data.Idioma)
            {
                case Enums.Idiomas.en_US:
                    Idioma = Recursos.Recurso.IdiomaIngles;
                    break;
                case Enums.Idiomas.es_ES:
                    Idioma = Recursos.Recurso.IdiomaEspanol;
                    break;
                case Enums.Idiomas.pt_BR:
                    Idioma = Recursos.Recurso.IdiomaPortugues;
                    break;
            }
            Result.Idioma = Idioma;

            return Result;
        }

        [HttpPost]
        public JsonResult BuscarPreguntas(ViewPreguntasFind Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<ViewPregunta> Result = new List<ViewPregunta>();
                Result = (from A in db.Pregunta
                          where A.Idioma == Data.Idioma &&
                          (Data.TipoControl == null || A.TipoControl == Data.TipoControl)
                          select new ViewPregunta
                          {
                              ClienteId = A.ClienteId,
                              Confirmacion = A.Confirmacion,
                              ControlId = A.ControlId,
                              Estado = A.Estado,
                              EstadoName = (A.Estado ? Recursos.Recurso.EstadoActivo : Recursos.Recurso.EstadoInactivo),
                              Id = A.Id,
                              Idioma = A.Idioma,
                              IdiomaName = (A.Idioma == 1 ? Recursos.Recurso.IdiomaEspanol : A.Idioma == 2 ? Recursos.Recurso.IdiomaIngles : Recursos.Recurso.IdiomaPortugues),
                              Texto = A.Texto,
                              TipoControl = A.TipoControl
                          }).ToList();

                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult BuscarAnidarPreguntas(ViewPreguntasFind Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<SelectListItem> Result = (from C in db.Pregunta
                                               where C.TipoControl == (int)Enums.TipoDeControl.ControlCerrado
                                               && C.Idioma == Data.Idioma
                                               select new SelectListItem
                                               {
                                                   Text = C.Texto,
                                                   Value = C.ControlId.ToString(),
                                               }).ToList();

                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult BuscarPreguntaAnidada(ViewPreguntasFind Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<SelectListItem> Result = (from C in db.Pregunta
                                               where C.TipoControl == (int)Enums.TipoDeControl.ControlCerrado
                                               && C.Idioma == Data.Idioma
                                               && C.ControlId != Data.ControlId
                                               select new SelectListItem
                                               {
                                                   Text = C.Texto,
                                                   Value = C.ControlId.ToString(),
                                                   Selected = (C.ControlId == Data.ChildControlId)
                                               }).ToList();

                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult BuscarValoreControl(ViewPreguntasFind Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ControlCerrado Result = db.ControlCerrado.Find(Data.ControlId);

                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}