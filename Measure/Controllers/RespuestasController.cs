using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Grupo;
using Measure.ViewModels.Respuesta;
using Measure.ViewModels.Usuario;
using Newtonsoft.Json;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class RespuestasController : Controller
    {
        [Route("Respuestas")]
        public ActionResult Index(Guid? Cliente)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("index", "Login");
            }
            ViewLogin Login = HttpContext.Session["login"] as ViewLogin;

            ViewResultAnswer modelo = new ViewResultAnswer
            {
                Clientes = new List<SelectListItem>()
            };

            using (ClsProcedures clsProcedures = new ClsProcedures())
            {
                Cliente = Cliente == Guid.Empty ? null : Cliente;
                modelo.Lista = clsProcedures.ListaRespuestas(Cliente);
            }

            if (Login.RolId == (int)Enums.UserRol.Administrador)
            {
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    modelo.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                        .Select(s => new SelectListItem
                        {
                            Text = s.Nombres,
                            Value = s.Id.ToString(),
                            Selected = (s.Id == Cliente)
                        }).ToList();
                }
            }
            return View(modelo);
        }

        [Route("FiltrarRespuestas")]
        [HttpPost]
        public ActionResult Respuesta(string Cliente)
        {
            return RedirectToRoute("Respuestas", new { Cliente = new Guid(Cliente) });
        }

        [Route("Respuesta")]
        [HttpPost]
        public ActionResult Respuesta(ViewReport Data)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;
            if (Usuario == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewBag.Layout = Usuario.RolId == (int)Enums.UserRol.Encuestado ? "~/Views/Shared/_LayoutUser.cshtml" : "~/Views/Shared/_Layout.cshtml";
            ViewPoll _Encuesta = new ViewPoll();

            using (ClsProcedures clsProcedures = new ClsProcedures())
            {                
                _Encuesta = clsProcedures.DetalleEncuestaUsuario(Data.Id, Usuario.Id);
            }

            ViewAnswerResult Modelo = new ViewAnswerResult();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Usuario Cliente = db.Usuario.Where(u => u.Id == _Encuesta.ClienteId).FirstOrDefault();

                List<ViewAnswerGroupPoll> Lista = new List<ViewAnswerGroupPoll>();

                List<Guid> IdGrupos = db.ContenidoPorEncuesta.
                    Where(c => c.EncuestaId == _Encuesta.id && c.ComponenteId != Guid.Empty).
                    OrderBy(o => o.Orden).Select(s => s.ComponenteId).ToList();

                List<Grupo> Grupos = new List<Grupo>();

                if (Data.Report)
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id)).ToList();
                }
                else
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id) && g.TipoReporte != 0 && g.TipoReporte != null).ToList();

                }

                List<Tuple<int, string>> ConsolidadoMatriz = new List<Tuple<int, string>>();

                foreach (Grupo item in Grupos)
                {
                    ViewAnswerGroupPoll Addtem = new ViewAnswerGroupPoll
                    {
                        Group = item,
                    };

                    switch (item.TipoReporte)
                    {
                        case 0:
                            Addtem.RespuestasAbierta = GraphicControlAbiero(Usuario, item, Data.Id);
                            Lista.Add(Addtem);
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            Addtem.RespuestasMatriz = GraphicControlMatrizRange(Usuario, item, Data.Id);
                            ConsolidadoMatriz.AddRange(Addtem.RespuestasMatriz.Select(l => new Tuple<int, string>(l.ResultQuestionMatrizRange.Radial.Serie, l.ResultQuestionMatrizRange.Radial.Label)).ToList());
                            Lista.Add(Addtem);
                            break;
                        case 4:
                            Modelo.Wellcome = GraphicPresentation(Usuario);
                            break;
                    }
                }

                Modelo.Cliente = Cliente;
                Modelo.ConsolidadoMatriz = ConsolidadoMatriz;
                Modelo.DataEncuesta = _Encuesta;
                Modelo.Grupos = Lista;
            }

            if (Data.Report)
            {
                string path = Server.MapPath("/Content/images/");
                List<string> Imagenes = new List<string>
                {
                    ConvertImagenPathFromBase64(path+"FondoBienvenida.png"),
                    ConvertImagenPathFromBase64(path+"foother_pdf.png"),
                    ConvertImagenPathFromBase64(path+(Usuario.Idioma == 1 ? "Dti_es.png": "Dti_en.png")),
                };
                ViewBag.Imagenes = Imagenes;
                return View("PDF", Modelo);
            }
            return View(Modelo);
        }

        [Route("Email")]
        [HttpGet]
        public ActionResult RespuestaEmail(ViewReport Data)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;
            if (Usuario == null)
            {
                return RedirectToAction("index", "Login");
            }

            ViewBag.Layout = Usuario.RolId == (int)Enums.UserRol.Encuestado ? "~/Views/Shared/_LayoutUser.cshtml" : "~/Views/Shared/_Layout.cshtml";
            ViewPoll _Encuesta = new ViewPoll();

            using (ClsProcedures clsProcedures = new ClsProcedures())
            {                
                _Encuesta = clsProcedures.DetalleEncuestaUsuario(Data.Id, Usuario.Id);
            }

            ViewAnswerResult Modelo = new ViewAnswerResult();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Usuario Cliente = db.Usuario.Where(u => u.Id == _Encuesta.ClienteId).FirstOrDefault();

                List<ViewAnswerGroupPoll> Lista = new List<ViewAnswerGroupPoll>();

                List<Guid> IdGrupos = db.ContenidoPorEncuesta.
                    Where(c => c.EncuestaId == _Encuesta.id && c.TipoComponente == (int)Enums.TipoComponente.CategoriaEncuesta).
                    OrderBy(o => o.Orden).Select(s => s.ComponenteId).ToList();

                List<Grupo> Grupos = new List<Grupo>();

                if (Data.Report)
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id) && g.ClienteId != Guid.Empty).ToList();
                }
                else
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id) && g.ClienteId != Guid.Empty && g.TipoReporte != 0 && g.TipoReporte != null).ToList();

                }

                List<Tuple<int, string>> ConsolidadoMatriz = new List<Tuple<int, string>>();

                foreach (Grupo item in Grupos)
                {
                    ViewAnswerGroupPoll Addtem = new ViewAnswerGroupPoll
                    {
                        Group = item,
                    };

                    switch (item.TipoReporte)
                    {
                        case 0:
                            Addtem.RespuestasAbierta = GraphicControlAbiero(Usuario, item, Data.Id);
                            Lista.Add(Addtem);
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            Addtem.RespuestasMatriz = GraphicControlMatrizRange(Usuario, item, Data.Id);
                            ConsolidadoMatriz.AddRange(Addtem.RespuestasMatriz.Select(l => new Tuple<int, string>(l.ResultQuestionMatrizRange.Radial.Serie, l.ResultQuestionMatrizRange.Radial.Label)).ToList());
                            Lista.Add(Addtem);
                            break;
                        case 4:
                            Modelo.Wellcome = GraphicPresentation(Usuario);
                            break;
                    }
                }

                Modelo.Cliente = Cliente;
                Modelo.ConsolidadoMatriz = ConsolidadoMatriz;
                Modelo.DataEncuesta = _Encuesta;
                Modelo.Grupos = Lista;
            }

            string path = Server.MapPath("/Content/images/");
            List<string> Imagenes = new List<string>
                {
                    ConvertImagenPathFromBase64(path+"FondoBienvenida.png"),
                    ConvertImagenPathFromBase64(path+"foother_pdf.png"),
                    ConvertImagenPathFromBase64(path+(Usuario.Idioma == 1 ? "Dti_es.png": "Dti_en.png")),
                };
            ViewBag.Imagenes = Imagenes;
            Modelo.Email = true;
            Modelo.Report = Data.Report;
            return View("PDF", Modelo);
        }

        [HttpPost]
        [Route("GeneratePdf")]
        public FileContentResult GeneratePdf(ViewDownload Data)
        {
            if (Data.Mac)
            {
                return File(ConvertHtmlToPdf(Data.Contenido), "application/pdf");
            }
            return File(ConvertHtmlToPdf(Data.Contenido), "application/pdf", ConfigurationManager.AppSettings["FileName"].ToString());
        }

        [HttpPost]
        [Route("SendEMail")]
        public ActionResult SendEmail(ViewDownload Data)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;

            SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            ViewPoll DataEncuesta = JsonConvert.DeserializeObject<ViewPoll>(Data.DataEncuesta);

            string mailbody = MailBody((Enums.Idiomas)Usuario.Idioma, Usuario);

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(smtpSection.Network.UserName.ToString());
                message.To.Add(Usuario.Correo);
                message.To.Add(ConfigurationManager.AppSettings["SecundaryEmail"].ToString());
                message.Subject = Recursos.Recurso.SujetoCorreo;
                message.Body = mailbody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                Byte[] DataHtml = ConvertHtmlToPdf(Data.Contenido);
                Stream DataStrem = new MemoryStream(DataHtml);
                message.Attachments.Add(new Attachment(DataStrem, ConfigurationManager.AppSettings["FileName"].ToString()));

                using (SmtpClient client = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    try
                    {
                        client.Send(message);

                        if (Data.Report)
                        {
                            return RedirectToAction("ResponderEncuesta", "Encuestas");
                        }
                        else
                        {
                            return RedirectToAction("MisEncuestas", "Encuestas", new { Id = Usuario.Id });
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        throw;
                    }
                }
            }
        }

        private string ConvertImagenPathFromBase64(string Path)
        {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return "data:image/png;base64," + base64String;
                }
            }
        }

        private Byte[] ConvertHtmlToPdf(string Html)
        {
            HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.Letter;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            PdfDocument document = converter.ConvertHtmlString(Html);
            document.Margins.Top = 2;
            document.Margins.Right = 2;
            document.Margins.Bottom = 2;
            document.Margins.Left = 2;

            string path = Server.MapPath("/Content/fonts/");
            PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(path + "Avenir-Roman.ttf");
            collection.AddFontFile(path + "BarlowCondensed-Regular.ttf");
            collection.AddFontFile(path + "Ubuntu-Bold.ttf");

            Font TitleFont = new Font(collection.Families[0], 14);
            document.AddFont(TitleFont);
            Font PageFont = new Font(collection.Families[1], 14);
            document.AddFont(PageFont);
            Font TextFont = new Font(collection.Families[2], 14);
            document.AddFont(TextFont);

            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            document.Close();
            return stream.ToArray();
        }

        private List<ViewAsnwerQuestionOpen> GraphicControlAbiero(ViewLogin Usuario, Grupo grupo, Guid IdAsignacion)
        {
            List<ViewAsnwerQuestionOpen> Result = new List<ViewAsnwerQuestionOpen>();
            List<Pregunta> _Preguntas = new List<Pregunta>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<Guid> IdPreguntas = db.PreguntasPorGrupo.Where(p => p.GrupoId == grupo.Id).OrderBy(o => o.Orden).Select(s => s.Id).ToList();
                _Preguntas = db.Pregunta.Where(p => IdPreguntas.Contains(p.Id) && p.Idioma == Usuario.Idioma).ToList();
            }

            foreach (Pregunta item in _Preguntas)
            {
                Respuesta _respuesta = new Respuesta();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    _respuesta = db.Respuesta.Where(r => r.PreguntaId == item.Id && r.UsuarioId == Usuario.Id && r.IdAsignacion == IdAsignacion).FirstOrDefault();
                }

                if (_respuesta != null)
                {
                    ViewAsnwerQuestionOpen Respuesta = new ViewAsnwerQuestionOpen
                    {
                        Question = item,
                    };

                    byte[] TextoBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(item.Texto);
                    string TextoStr = Encoding.UTF8.GetString(TextoBytes);
                    byte[] ResultadoBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(_respuesta.Resultado);
                    string ResultadoStr = Encoding.UTF8.GetString(ResultadoBytes);

                    switch ((Enums.TipoDeControl)item.TipoControl)
                    {
                        case Enums.TipoDeControl.ControlAbierto:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Respuesta.ControlA = db.ControlAbierto.Where(c => c.Id == item.ControlId).FirstOrDefault();
                            }
                            switch ((Enums.ControlAbierto)Respuesta.ControlA.TipoAbierta)
                            {
                                case Enums.ControlAbierto.Ca_Entrada:
                                    Respuesta.Contenido = new Tuple<Guid, string, string, string>(item.Id, TextoStr, ResultadoStr, string.Empty);
                                    break;
                                case Enums.ControlAbierto.Ca_Entrada_Descripcion:
                                    Respuesta.Contenido = new Tuple<Guid, string, string, string>(item.Id, Respuesta.ControlA.DescripcionPre, ResultadoStr, Respuesta.ControlA.DescripcionPos);
                                    break;
                                case Enums.ControlAbierto.Ca_Rango:
                                    break;
                                case Enums.ControlAbierto.Ca_Entrada_Multiple:
                                    break;
                                case Enums.ControlAbierto.Ca_Texto_Multiple:
                                    break;
                            }
                            Result.Add(Respuesta);

                            break;
                        case Enums.TipoDeControl.ControlCerrado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Respuesta.ControlC = db.ControlCerrado.Where(c => c.Id == item.ControlId).FirstOrDefault();
                            }
                            Respuesta.Contenido = new Tuple<Guid, string, string, string>(item.Id, TextoStr, ResultadoStr, string.Empty);
                            Result.Add(Respuesta);

                            break;
                    }
                }
            }

            return Result;
        }

        private List<ViewAsnwerQuestionMatriz> GraphicControlMatrizRange(ViewLogin Usuario, Grupo grupo, Guid IdAsignacion)
        {
            List<ViewAsnwerQuestionMatriz> Result = new List<ViewAsnwerQuestionMatriz>();

            List<Pregunta> _Preguntas = new List<Pregunta>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<Guid> IdPreguntas = db.PreguntasPorGrupo.Where(p => p.GrupoId == grupo.Id).OrderBy(o => o.Orden).Select(s => s.PreguntaId).ToList();
                _Preguntas = db.Pregunta.Where(p => IdPreguntas.Contains(p.Id) && p.Idioma == Usuario.Idioma).ToList();
            }

            foreach (Pregunta item in _Preguntas)
            {
                ControlMatriz control = new ControlMatriz();
                Respuesta _respuesta = new Respuesta();
                using (ModeloEncuesta db = new ModeloEncuesta())
                {
                    control = db.ControlMatriz.Where(c => c.Id == item.ControlId).FirstOrDefault();
                    _respuesta = db.Respuesta.Where(r => r.PreguntaId == item.Id && r.UsuarioId == Usuario.Id && r.IdAsignacion == IdAsignacion).FirstOrDefault();
                }

                if (_respuesta != null)
                {
                    ViewAsnwerQuestionMatriz Respuesta = new ViewAsnwerQuestionMatriz
                    {
                        Question = item,
                        ControlM = control
                    };

                    switch ((Enums.ControlMatriz)control.TipoControl)
                    {
                        case Enums.ControlMatriz.radio:
                            break;
                        case Enums.ControlMatriz.checkbox:
                            break;
                        case Enums.ControlMatriz.range:
                            List<string> SubPreguntas = new List<string>();
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                SubPreguntas = db.ControlMatrizFila.Where(c => c.MatrizId == control.Id).Select(s => s.Texto).ToList();
                            }

                            int Calificacion = 0;
                            List<string> Respuestas = _respuesta == null ? new List<string>() : _respuesta.Resultado.Split('|').ToList();
                            List<Tuple<int, string>> Contenido = new List<Tuple<int, string>>();

                            for (int a = 0; a < Respuestas.Count(); a++)
                            {
                                int IntItem = Convert.ToInt32(Respuestas[a]);
                                Calificacion += IntItem;
                                Contenido.Add(new Tuple<int, string>(IntItem, SubPreguntas[a]));
                            }

                            Respuesta.ResultQuestionMatrizRange = new ViewResultMatrizRange
                            {
                                Barra = Contenido,
                                Radial = new RadialBar
                                {
                                    Label = Usuario.Idioma == (int)Enums.Idiomas.es_ES ? grupo.es_Es : Usuario.Idioma == (int)Enums.Idiomas.en_US ? grupo.en_US : grupo.pt_BR,
                                    Serie = Calificacion
                                }
                            };

                            break;
                    }
                    Result.Add(Respuesta);
                }
            }



            return Result;
        }

        private ViewPresentation GraphicPresentation(ViewLogin Usuario)
        {
            Usuario User = new Usuario();
            Usuario Client = new Usuario();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                User = db.Usuario.Find(Usuario.Id);
                Client = db.Usuario.Find(User.ClienteId);
            }

            string DataJoin = "{0} {1}";

            return new ViewPresentation
            {
                NombreEmpresa = string.Format(DataJoin, Client.Nombres, Client.Apellidos),
                Nombres = string.Format(DataJoin, User.Nombres, User.Apellidos),
            };
        }

        private string MailBody(Enums.Idiomas Idioma, ViewLogin Usuario)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Usuario Result = db.Usuario.Find(Usuario.Id);
                MaestrasDetalle Empleados = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Empleados")).MaestrasDetalle.FirstOrDefault(m => m.Id == Result.CEmpleadosId);
                MaestrasDetalle Industrias = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.FirstOrDefault(m => m.Valor == Result.Idioma.ToString());
                MaestrasDetalle Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.FirstOrDefault(m => m.Valor == Result.PaisId.ToString());

                return string.Format(Recursos.Recurso.CuerpoCorreo,
                                    Result.Nombres, Result.Apellidos, Result.Empresa,
                                    (Idioma == Enums.Idiomas.es_ES ? Empleados.es_ES : Idioma == Enums.Idiomas.en_US ? Empleados.en_US : Empleados.pt_BR),
                                    Result.Titulo,
                                    (Idioma == Enums.Idiomas.es_ES ? Industrias.es_ES : Idioma == Enums.Idiomas.en_US ? Industrias.en_US : Industrias.pt_BR),
                                    Result.Correo, Result.Telefono,
                                    (Idioma == Enums.Idiomas.es_ES ? Paises.es_ES : Idioma == Enums.Idiomas.en_US ? Paises.en_US : Paises.pt_BR)
                                    );
            }

        }
    }
}