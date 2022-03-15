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
            Usuario Cliente = new Usuario();
            List<Grupo> Grupos = new List<Grupo>();
            List<ViewAnswerGroupPoll> Lista = new List<ViewAnswerGroupPoll>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Cliente = db.Usuario.Where(u => u.Id == _Encuesta.ClienteId).FirstOrDefault();

                List<ContenidoPorEncuesta> TempGroups = db.ContenidoPorEncuesta.
                    Where(c => c.EncuestaId == _Encuesta.id && c.ComponenteId != Guid.Empty).
                    OrderBy(o => o.Orden).ToList();

                foreach (ContenidoPorEncuesta item in TempGroups)
                {
                    Grupo AddItem = new Grupo();
                    if (Data.Report)
                    {
                        AddItem = db.Grupo.FirstOrDefault(g => g.Id == item.ComponenteId);
                    }
                    else
                    {
                        AddItem = db.Grupo.FirstOrDefault(g => g.Id == item.ComponenteId && g.TipoReporte != 0 && g.TipoReporte != null);
                    }
                    if (AddItem != null)
                    {
                        Grupos.Add(AddItem);
                    }                    
                }                
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

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.ContenidoReporte = db.Reporte.Find(_Encuesta.TipoReporteGeneral)
                                          .ContenidosReporte.Where(c => c.Idioma == Usuario.Idioma && c.Estado).ToList();
            }

            string path = Server.MapPath("/Content/images/");
            List<string> Imagenes = new List<string>
                {
                    ConvertImagenPathFromBase64(path+"FondoBienvenida.png"),
                    ConvertImagenPathFromBase64(path+"foother_pdf.png"),
                    ConvertImagenPathFromBase64(path+(Usuario.Idioma == 1 ? "Dti_es.png": "Dti_en.png")),
                };
            ViewBag.Imagenes = Imagenes;

            if (Data.Report)
            {
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
            Usuario Cliente = new Usuario();
            List<Grupo> Grupos = new List<Grupo>();
            List<ViewAnswerGroupPoll> Lista = new List<ViewAnswerGroupPoll>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Cliente = db.Usuario.Where(u => u.Id == _Encuesta.ClienteId).FirstOrDefault();

                List<Guid> IdGrupos = db.ContenidoPorEncuesta.
                    Where(c => c.EncuestaId == _Encuesta.id && c.ComponenteId != Guid.Empty).
                    OrderBy(o => o.Orden).Select(s => s.ComponenteId).ToList();

                if (Data.Report)
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id)).ToList();
                }
                else
                {
                    Grupos = db.Grupo.Where(g => IdGrupos.Contains(g.Id) && g.TipoReporte != 0 && g.TipoReporte != null).ToList();
                }
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

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Modelo.ContenidoReporte = db.Reporte.Find(_Encuesta.TipoReporteGeneral)
                                          .ContenidosReporte.Where(c => c.Idioma == Usuario.Idioma && c.Estado).ToList();
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
            string mailbody = MailBody((Enums.Idiomas)Usuario.Idioma, Usuario);
            byte[] DataHtml = ConvertHtmlToPdf(Data.Contenido);

            bool send = EmailSending(Usuario.Correo, DataHtml, mailbody);
            send = EmailSending(ConfigurationManager.AppSettings["SecundaryEmail"].ToString(), DataHtml, mailbody);

            if (Data.Report)
            {
                return RedirectToAction("ResponderEncuesta", "Encuestas");
            }
            else
            {
                return RedirectToAction("MisEncuestas", "Encuestas", new { Id = Usuario.Id });
            }
        }

        [HttpGet]
        [Route("PruebaEmail")]
        public ActionResult PruebaEmail(string Email)
        {
            ViewLogin Usuario = new ViewLogin
            {
                Idioma = 1,
                Correo = Email,
                Nombres = "Rene Alejandro",
                Apellidos = "Paramo Porras",
                Empresa = "Prueba",
                Titulo = "Desarrollador",
                Industria = "Desarrollo",
                Pais = "Colombia"
            };
            string Contenido = "<!DOCTYPE html> <html lang='es'> <head> <meta charset='UTF-8'>" +
                " <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta name='viewport' " +
                "content='width=device-width, initial-scale=1.0'> <title>Correo</title> </head> " +
                "<body style='font-family: Arial, Helvetica, sans-serif;margin: 20px;'>" +
                " <div>Estimado usuario de CONSULTREE</div> " +
                "<div style='padding-top: 10px;'>Se adjunta el informe de la evaluación DTI para :</div></body> </html>";
            string mailbody = MailBody((Enums.Idiomas)Usuario.Idioma, Usuario);
            byte[] DataHtml = ConvertHtmlToPdf(Contenido);

            bool send = EmailSending(Usuario.Correo, DataHtml, mailbody);
            return RedirectToAction("index", "Login");
        }

        private bool EmailSending(string Email, byte[] DataHtml, string mailbody)
        {
            bool Result = true;
            try
            {
                SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(smtpSection.Network.UserName.ToString());
                    message.To.Add(Email);
                    message.Subject = Recursos.Recurso.SujetoCorreo;
                    message.Body = mailbody;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;

                    Stream DataStrem = new MemoryStream(DataHtml);
                    message.Attachments.Add(new Attachment(DataStrem, ConfigurationManager.AppSettings["FileName"].ToString()));

                    using (SmtpClient client = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;

                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                Result = false;
            }
            return Result;
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
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.Letter;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            converter.Options.MarginLeft = 0;
            converter.Options.MarginRight = 0;
            converter.Options.MarginTop = 0;
            converter.Options.MarginBottom = 0;

            PdfDocument document = converter.ConvertHtmlString(Html);
            document.Margins.Top = 2;
            document.Margins.Right = 2;
            document.Margins.Bottom = 2;
            document.Margins.Left = 2;

            string pathFnt = Server.MapPath("/Content/fonts/");
            PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(pathFnt + "BarlowCondensed-Regular.ttf");
            collection.AddFontFile(pathFnt + "Ubuntu-Bold.ttf");

            Font PageFont = new Font(collection.Families[0], 14);
            document.AddFont(PageFont, true);
            Font TextFont = new Font(collection.Families[1], 14);
            document.AddFont(TextFont, true);

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
                _Preguntas = db.Pregunta.Where(p => IdPreguntas.Contains(p.Id) && p.Idioma == Usuario.Idioma && p.Estado).OrderBy(o => o.Texto).ToList();
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
                                SubPreguntas = db.ControlMatrizFila.Where(c => c.MatrizId == control.Id).OrderBy(o => o.Orden).Select(s => s.Texto).ToList();
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