using Measure.Enums;
using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Error;
using Measure.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class LoginController : Controller
    {
        public string Message { get; set; }
        public ActionResult Index(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            }
            AssignLanguage(culture);

            List<SelectListItem> lenguajes = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = Recursos.Recurso.IdiomaEspanol, Selected = (culture == "es" || culture == "es-ES") },
                new SelectListItem { Value = "2", Text = Recursos.Recurso.IdiomaIngles, Selected = (culture == "en" || culture == "en-US") },
                new SelectListItem { Value = "3", Text = Recursos.Recurso.IdiomaPortugues, Selected = (culture == "pt" || culture == "pt-BR") }
            };

            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;
            ViewLogin data = new ViewLogin
            {
                Lenguajes = lenguajes
            };

            if (HttpContext.Session["login"] != null)
            {
                data.Error = Usuario.Error;
                data.Mensaje = Usuario.Mensaje;

                Usuario.Error = false;
                Usuario.Mensaje = string.Empty;
                HttpContext.Session["login"] = Usuario;
            }
            else
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    data.Error = true;
                    data.Mensaje = Message;
                    Message = string.Empty;
                }
            }

            return View(data);
        }

        [HttpGet]
        public JsonResult Unloock(string Old)
        {
            string Result = string.Empty;
            using (ClsUtilities clsUtilities = new ClsUtilities())
            {
                Result = clsUtilities.Cifrado(Old, false);
            }
            return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LanguajeLogin(ViewLogin Usuario)
        {
            return RedirectToAction("Index", "Login", new { culture = Usuario.Lenguaje.ToString().Replace("_", "-") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ViewLogin Usuario)
        {
            ViewLogin Login = new ViewLogin();

            if (!string.IsNullOrEmpty(Usuario.Clave))
            {
                Usuario User = new Usuario();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    try
                    {
                        User = db.Usuario.FirstOrDefault(u => u.Correo.Equals(Usuario.Correo));
                        if (User != null)
                        {
                            using (ClsUtilities clsUtilities = new ClsUtilities())
                            {
                                string clave = clsUtilities.Cifrado(Usuario.Clave, true);

                                if (User.Clave == clave)
                                {
                                    Login = ResultViewLogin(User);
                                    clsUtilities.CrearCache("login", Login);
                                    AssignLanguage(((Enums.Idiomas)User.Idioma).ToString());

                                    if (Login.RolId == (int)UserRol.Encuestado)
                                    {
                                        return RedirectToAction("MisEncuestas", "Encuestas", new { Id = User.Id });
                                    }
                                    else
                                    {
                                        return RedirectToRoute("Home");
                                    }
                                }
                                else
                                {
                                    Login = new ViewLogin
                                    {
                                        Error = true,
                                        Mensaje = Recursos.Recurso.LoginError4,
                                    };
                                }
                            }
                        }
                        else
                        {
                            Login = new ViewLogin
                            {
                                Error = true,
                                Mensaje = Recursos.Recurso.LoginSinUsuario
                            };
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        using (ClsUtilities ClaseUtil = new ClsUtilities())
                        {
                            ViewCatchError Error = ClaseUtil.CatchException(e);
                            Login = new ViewLogin
                            {
                                Error = Error.Error,
                                Mensaje = string.Join("\n", Error.Mensajes)
                            };
                        }
                    }
                }
            }
            else
            {
                Login = new ViewLogin
                {
                    Error = true,
                    Mensaje = Recursos.Recurso.LoginSinClave
                };
            }

            Login.Lenguajes = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = Recursos.Recurso.IdiomaEspanol, Selected = (((int)Usuario.Lenguaje).ToString() == "0") },
                new SelectListItem { Value = "1", Text = Recursos.Recurso.IdiomaIngles, Selected = (((int)Usuario.Lenguaje).ToString() == "1") },
                new SelectListItem { Value = "2", Text = Recursos.Recurso.IdiomaPortugues, Selected = (((int)Usuario.Lenguaje).ToString() == "2") }
            };

            return View(Login);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Message = Recursos.Recurso.LoginCerrarSesion;
            using (ClsUtilities utilidades = new ClsUtilities())
            {
                utilidades.EliminarUnicoCache("login");
            }
            return RedirectToAction("Index");
        }

        private ViewLogin ResultViewLogin(Usuario User)
        {
            ViewLogin Result = new ViewLogin
            {
                Apellidos = User.Apellidos,
                Clave = User.Clave,
                Correo = User.Correo,
                Estado = User.Estado,
                Id = User.Id,
                Idioma = User.Idioma,
                Nombres = User.Nombres,
                RolId = User.RolId,
            };

            if (User.ClienteId != null)
            {
                Result.ClienteId = User.ClienteId;
                if (Result.RolId == (int)UserRol.Encuestado)
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Usuario aliado = db.Usuario.Where(u => u.Id == (Guid)User.AliadoId).FirstOrDefault();
                        Usuario Cliente = db.Usuario.Where(u => u.Id == (Guid)aliado.ClienteId).FirstOrDefault();
                        Result.Cliente = Cliente.Nombres;
                    }
                }
                else if (Result.RolId == (int)UserRol.Aliado)
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Usuario Cliente = db.Usuario.Where(u => u.Id == (Guid)User.ClienteId).FirstOrDefault();
                        Result.Cliente = Cliente.Nombres;
                    }
                }

            }

            if (User.PaisId != null)
            {
                Result.PaisId = User.PaisId;
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    Maestras Paises = db.Maestras.Where(m => m.ClienteId == Guid.Empty && m.es_ES == "Pais").FirstOrDefault();
                    MaestrasDetalle Pais = Paises.MaestrasDetalle.Where(m => m.Valor == User.PaisId.ToString()).FirstOrDefault();
                    switch ((Idiomas)User.Idioma)
                    {
                        case Idiomas.es_ES:
                            Result.Pais = Pais.es_ES;
                            break;
                        case Idiomas.en_US:
                            Result.Pais = Pais.en_US;
                            break;
                        case Idiomas.pt_BR:
                            Result.Pais = Pais.pt_BR;
                            break;
                    }
                }
            }

            if (User.Imagen != null)
            {
                Result.Imagen = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(User.Imagen));
            }
            return Result;
        }

        private void AssignLanguage(string culture)
        {
            using (ClsUtilities ClaseConversion = new ClsUtilities())
            {
                switch (culture)
                {
                    case "es":
                    case "es-ES":
                    case "es_ES":
                    default:
                        ClaseConversion.CrearCache("BaseIdioma", Enums.Idiomas.es_ES);
                        System.Globalization.CultureInfo es = System.Globalization.CultureInfo.GetCultureInfo(Enums.Idiomas.es_ES.ToString().Replace("_", "-"));
                        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = es;
                        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = es;
                        System.Threading.Thread.CurrentThread.CurrentCulture = es;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = es;
                        break;
                    case "en":
                    case "en-US":
                    case "en_US":
                        ClaseConversion.CrearCache("BaseIdioma", Enums.Idiomas.en_US);
                        System.Globalization.CultureInfo en = System.Globalization.CultureInfo.GetCultureInfo(Enums.Idiomas.en_US.ToString().Replace("_", "-"));
                        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = en;
                        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = en;
                        System.Threading.Thread.CurrentThread.CurrentCulture = en;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = en;
                        break;
                    case "pt":
                    case "pt-BR":
                    case "pt_BR":
                        ClaseConversion.CrearCache("BaseIdioma", Enums.Idiomas.pt_BR);
                        System.Globalization.CultureInfo pt = System.Globalization.CultureInfo.GetCultureInfo(Enums.Idiomas.pt_BR.ToString().Replace("_", "-"));
                        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = pt;
                        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = pt;
                        System.Threading.Thread.CurrentThread.CurrentCulture = pt;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = pt;
                        break;
                }
            }

        }
    }
}