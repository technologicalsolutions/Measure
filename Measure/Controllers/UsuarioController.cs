﻿using Measure.Models;
using Measure.Utilidades;
using Measure.ViewModels.Encuesta;
using Measure.ViewModels.Error;
using Measure.ViewModels.Usuario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Measure.Controllers
{
    public class UsuarioController : Controller
    {
        [Route("Usuarios")]
        public ActionResult Index(int Rol, Guid? ClienteId)
        {
            ViewLogin _Usuario = HttpContext.Session["login"] as ViewLogin;
            if (_Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewUsuario Model = new ViewUsuario
            {
                FindRolId = _Usuario.RolId,
                Modelo = new ViewUser
                {
                    ClienteId = ClienteId,
                    RolId = Rol
                }
            };

            using (ClsProcedures procedures = new ClsProcedures())
            {
                Guid? AliadoId = null;
                if (ClienteId != Guid.Empty)
                {
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Models.Usuario User = db.Usuario.Find(ClienteId);
                        if (User.RolId == (int)Enums.UserRol.Aliado)
                        {
                            AliadoId = User.AliadoId;
                            ClienteId = User.ClienteId;
                        }
                    }
                }
                else
                {
                    ClienteId = null;
                }
                Model.Lista = procedures.UsuariosPorRol(Rol, ClienteId, AliadoId, _Usuario.Idioma, null);
            }

            switch ((Enums.UserRol)Rol)
            {
                case Enums.UserRol.Cliente:
                    ViewBag.adminShow = "show";
                    ViewBag.ClienteActive = "active";
                    break;
                case Enums.UserRol.Aliado:
                    ViewBag.userShow = "show";
                    ViewBag.AliadoActive = "active";
                    break;
                case Enums.UserRol.Encuestado:
                    ViewBag.userShow = "show";
                    ViewBag.EncuestadoActive = "active";
                    break;
            }
            ViewBag.BtnCreate = ((Enums.UserRol)Rol).ToString();
            return View(Model);
        }

        [HttpPost]
        [Route("Usuario")]
        public ActionResult Usuario(ViewUsuario Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                Data = CreateData(Data);
                ModelState.Clear();
            }
            return View("CreateOrEdit", Data);
        }

        private ViewUsuario CreateData(ViewUsuario Data)
        {
            Enums.Idiomas BasePlataforma = (Enums.Idiomas)HttpContext.Session["BaseIdioma"];

            SelectListItem Seleccione = new SelectListItem { Text = "Seleccione...", Value = "0" };
            Data.Clientes = new List<SelectListItem> { Seleccione };
            Data.Aliados = new List<SelectListItem> { Seleccione };
            Data.Idiomas = new List<SelectListItem> { Seleccione };
            Data.Paises = new List<SelectListItem> { Seleccione };

            List<MaestrasDetalle> Lenguaje = new List<MaestrasDetalle>();
            List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Lenguaje = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.ToList();
                Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.ToList();
            }

            switch (BasePlataforma)
            {
                case Enums.Idiomas.es_ES:
                    Data.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.es_ES, Value = s.Valor }).ToList());
                    break;
                case Enums.Idiomas.en_US:
                    Data.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.en_US, Value = s.Valor }).ToList());
                    break;
                case Enums.Idiomas.pt_BR:
                    Data.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.pt_BR, Value = s.Valor }).ToList());
                    break;
            }

            Data.Paises.AddRange(Paises.Select(s => new SelectListItem
            {
                Text = BasePlataforma == Enums.Idiomas.es_ES ? s.es_ES : BasePlataforma == Enums.Idiomas.en_US ? s.es_ES : s.pt_BR,
                Value = s.Valor.ToString()
            }).ToList());

            if (Data.Modelo.ClienteId == Guid.Empty || Data.Modelo.ClienteId == null)
            {
                if (Data.Accion == Enums.DbAcciones.Crea)
                {
                    ViewBag.Title = "Crear";
                    switch ((Enums.UserRol)Data.Modelo.RolId)
                    {
                        case Enums.UserRol.Aliado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Data.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                                .Select(s => new SelectListItem
                                {
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());
                            }
                            break;
                        case Enums.UserRol.Encuestado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Data.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                                .Select(s => new SelectListItem
                                {
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());

                                if (Data.Clientes.Count() > 1)
                                {
                                    Guid CrClienteId = new Guid(Data.Clientes[1].Value);
                                    Data.Aliados.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Aliado && u.ClienteId == CrClienteId)
                                    .Select(s => new SelectListItem
                                    {
                                        Text = s.Nombres,
                                        Value = s.Id.ToString()
                                    }).ToList());
                                }                                
                            }
                            break;
                    }
                }
                else if (Data.Accion == Enums.DbAcciones.Actualiza)
                {
                    ViewBag.Title = "Editar";
                    string Correo = Data.Modelo.Correo;
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Data.Modelo = ResultViewUser(db.Usuario.Where(u => u.Correo == Correo).FirstOrDefault());
                    }

                    Data.Idiomas = Data.Idiomas.Select(s => new SelectListItem
                    {
                        Selected = (s.Value == Data.Modelo.Idioma.ToString()),
                        Text = s.Text,
                        Value = s.Value
                    }).ToList();

                    if (Data.Modelo.PaisId != null)
                    {                        
                        Data.Paises = Data.Paises.Select(s => new SelectListItem
                        {
                            Selected = (s.Value == Data.Modelo.PaisId.ToString()),
                            Text = s.Text,
                            Value = s.Value
                        }).ToList();
                    }

                    switch ((Enums.UserRol)Data.Modelo.RolId)
                    {
                        case Enums.UserRol.Aliado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Data.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                                .Select(s => new SelectListItem
                                {
                                    Selected = (Data.Modelo.ClienteId == s.Id),
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());
                            }
                            break;
                        case Enums.UserRol.Encuestado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Data.Clientes.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                                .Select(s => new SelectListItem
                                {
                                    Selected = (Data.Modelo.ClienteId == s.Id),
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());

                                if (Data.Clientes.Count() > 1)
                                {
                                    Guid CrClienteId = new Guid(Data.Clientes[1].Value);
                                    Data.Aliados.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Aliado && u.ClienteId == CrClienteId)
                                    .Select(s => new SelectListItem
                                    {
                                        Selected = (Data.Modelo.AliadoId == s.Id),
                                        Text = s.Nombres,
                                        Value = s.Id.ToString()
                                    }).ToList());
                                }
                            }
                            break;
                    }
                }
            }
            else
            {                
                if (Data.Accion == Enums.DbAcciones.Crea)
                {
                    ViewBag.Title = "Crear";
                    switch ((Enums.UserRol)Data.Modelo.RolId)
                    {
                        case Enums.UserRol.Encuestado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Guid CrClienteId = (Guid)Data.Modelo.ClienteId;
                                Data.Aliados.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Aliado && u.ClienteId == CrClienteId)
                                .Select(s => new SelectListItem
                                {
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());
                            }
                            break;
                    }
                }
                else if (Data.Accion == Enums.DbAcciones.Actualiza)
                {
                    ViewBag.Title = "Editar";
                    string Correo = Data.Modelo.Correo;
                    using (ModeloEncuesta db = new ModeloEncuesta())
                    {
                        Data.Modelo = ResultViewUser(db.Usuario.Where(u => u.Correo == Correo).FirstOrDefault());
                    }

                    Data.Idiomas = Data.Idiomas.Select(s => new SelectListItem
                    {
                        Selected = (s.Value == Data.Modelo.Idioma.ToString()),
                        Text = s.Text,
                        Value = s.Value
                    }).ToList();

                    if (Data.Modelo.PaisId != null)
                    {
                        Data.Paises = Data.Paises.Select(s => new SelectListItem
                        {
                            Selected = (s.Value == Data.Modelo.PaisId.ToString()),
                            Text = s.Text,
                            Value = s.Value
                        }).ToList();
                    }

                    switch ((Enums.UserRol)Data.Modelo.RolId)
                    {
                        case Enums.UserRol.Encuestado:
                            using (ModeloEncuesta db = new ModeloEncuesta())
                            {
                                Guid CrClienteId = (Guid)Data.Modelo.ClienteId;
                                Data.Aliados.AddRange(db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Aliado && u.ClienteId == CrClienteId)
                                .Select(s => new SelectListItem
                                {
                                    Selected = (Data.Modelo.AliadoId == s.Id),
                                    Text = s.Nombres,
                                    Value = s.Id.ToString()
                                }).ToList());
                            }
                            break;
                    }
                }
            }

            return Data;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UserAcciones")]
        public ActionResult Acciones(ViewUsuario Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                ModelState.Clear();
                bool validar = true;

                if (string.IsNullOrEmpty(Data.Modelo.Clave))
                {
                    ModelState.AddModelError("", "La clave no puede ir vacia.");
                    validar = false;
                }

                if (Data.Modelo.Idioma == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un idioma");
                    validar = false;
                }

                if (string.IsNullOrEmpty(Data.Modelo.Nombres))
                {
                    ModelState.AddModelError("", "Debe de escribir un nombre");
                    validar = false;
                }

                if (Data.Modelo.PaisId == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un pais");
                    validar = false;
                }

                if (Data.Accion == Enums.DbAcciones.Crea)
                {
                    if (db.Usuario.Count(u => u.Correo == Data.Modelo.Correo) > 0)
                    {
                        ModelState.AddModelError("", "Debe escribir un correo que no se ecuentre registrado");
                        validar = false;
                    }
                }


                if (!validar)
                {
                    Data = CreateData(Data);
                    return View("CreateOrEdit", Data);
                }
                try
                {
                    Usuario User = ResultUser(Data.Modelo);
                    switch (Data.Accion)
                    {
                        case Enums.DbAcciones.Crea:
                            using (ClsUtilities clsUtilities = new ClsUtilities())
                            {
                                string pass = clsUtilities.Cifrado(User.Clave, true);
                                User.Clave = pass;
                            }
                            db.Usuario.Add(User);
                            break;
                        case Enums.DbAcciones.Actualiza:
                            Usuario _OldUser = db.Usuario.Where(u => u.Correo == User.Correo).FirstOrDefault();
                            Usuario OldUser = CompareUser(_OldUser, User);
                            db.Entry(OldUser).State = EntityState.Modified;
                            break;
                    }
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    using (ClsUtilities ClaseUtil = new ClsUtilities())
                    {
                        ViewCatchError Error = ClaseUtil.CatchException(e);
                        return new JsonResult { Data = Error, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                return RedirectToRoute("Home");
            }
        }

        [HttpPost]
        public PartialViewResult Detail(ViewUsuario Data)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                string Correo = Data.Modelo.Correo;
                Data.Modelo = ResultViewUser(db.Usuario.Where(u => u.Correo == Correo).FirstOrDefault());
            }
            return PartialView("_Details", Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Correo)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Usuario usuario = db.Usuario.Where(u => u.Correo == Correo).First();
                db.Usuario.Remove(usuario);
                db.SaveChanges();
                return RedirectToRoute("Home");
            }
        }

        public ViewUser ResultViewUser(Usuario Modelo)
        {
            ViewUser Result = new ViewUser
            {
                Apellidos = Modelo.Apellidos,
                AliadoId = Modelo.AliadoId,
                Clave = Modelo.Clave,
                ClienteId = Modelo.ClienteId,
                Color = Modelo.Color,
                Correo = Modelo.Correo,
                Estado = Modelo.Estado,
                Id = Modelo.Id,
                Idioma = Modelo.Idioma,
                Nombres = Modelo.Nombres,                
                RolId = Modelo.RolId,                
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (Modelo.ClienteId != null)
                {
                    Result.Cliente = db.Usuario.Where(u => u.Id == Modelo.ClienteId).FirstOrDefault().Nombres;
                }
                if (Modelo.Imagen != null)
                {
                    Result.Imagen = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Modelo.Imagen));
                }
                if (Modelo.PaisId != null)
                {
                    Maestras _Maestra = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais"));
                    MaestrasDetalle _pais = _Maestra.MaestrasDetalle.FirstOrDefault(p => p.Id == Modelo.PaisId);
                    Result.Pais = Modelo.Idioma == (int)Enums.Idiomas.es_ES ? _pais.es_ES : Modelo.Idioma == (int)Enums.Idiomas.en_US ? _pais.en_US : _pais.pt_BR;
                }

            }
            return Result;
        }

        public Usuario ResultUser(ViewUser Modelo)
        {
            Usuario Result = new Usuario
            {
                Apellidos = Modelo.Apellidos,
                Clave = Modelo.Clave,
                ClienteId = Modelo.ClienteId,
                Color = Modelo.Color,
                Correo = Modelo.Correo,
                Estado = Modelo.Estado,
                Id = Modelo.Id == null ? Guid.NewGuid() : (Guid)Modelo.Id,
                Idioma = Modelo.Idioma,
                Nombres = Modelo.Nombres,
                PaisId = Modelo.PaisId,
                RolId = Modelo.RolId,
                Telefono = Modelo.Telefono
            };

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (Modelo.Image != null)
                {
                    byte[] data;
                    using (Stream inputStream = Modelo.Image.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                    Result.Imagen = data;
                }
            }
            return Result;
        }

        private Usuario CompareUser(Usuario UserOld, Usuario UserNew)
        {
            UserOld.Apellidos = UserOld.Apellidos != UserNew.Apellidos ? UserNew.Apellidos : UserOld.Apellidos;

            using (ClsUtilities clsUtilities = new ClsUtilities())
            {
                if (UserNew.Clave != UserOld.Clave)
                {
                    string pass = clsUtilities.Cifrado(UserNew.Clave, true);
                    UserOld.Clave = pass;
                }
            }
            
            UserOld.Color = UserOld.Color != UserNew.Color ? UserNew.Color : UserOld.Color;
            UserOld.Estado = UserOld.Estado != UserNew.Estado ? UserNew.Estado : UserOld.Estado;
            UserOld.Idioma = UserOld.Idioma != UserNew.Idioma ? UserNew.Idioma : UserOld.Idioma;
            if (UserNew.Imagen != null)
            {
                UserOld.Imagen = UserOld.Imagen != UserNew.Imagen ? UserNew.Imagen : UserOld.Imagen;
            }
            UserOld.Nombres = UserOld.Nombres != UserNew.Nombres ? UserNew.Nombres : UserOld.Nombres;
            UserOld.PaisId = UserOld.PaisId != UserNew.PaisId ? UserNew.PaisId : UserOld.PaisId;

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (UserOld.RolId != UserNew.RolId)
                {
                    int _Encuestas = db.Encuesta.Count(e => e.ClienteId == UserOld.Id);
                    if (_Encuestas == 0)
                    {
                        UserOld.RolId = UserNew.RolId;
                    }
                }

            }

            UserOld.Telefono = UserOld.Telefono != UserNew.Telefono ? UserNew.Telefono : UserOld.Telefono;

            return UserOld;
        }

        [HttpPost]
        [Route("BuscarUsuarios")]
        public ActionResult BuscarUsuarios(Guid id)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;
            if (Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            Enums.Idiomas BasePlataforma = (Enums.Idiomas)HttpContext.Session["BaseIdioma"];
            SelectListItem Seleccione = new SelectListItem { Text = "Seleccione...", Value = "0" };

            ViewPollAssign Model = new ViewPollAssign
            {
                Clientes = new List<SelectListItem>(),
                Idiomas = new List<SelectListItem> { Seleccione },
                Modelo = new ViewFindPollAssign(),
                Paises = new List<SelectListItem> { Seleccione },
                Usuarios = new List<ViewUser>()
            };

            List<MaestrasDetalle> Lenguaje = new List<MaestrasDetalle>();
            List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Lenguaje = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.ToList();
                Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.ToList();
                Model.DataEncuesta = db.Encuesta.Find(id);

                if (Usuario.RolId == (int)Enums.UserRol.Administrador)
                {
                    Model.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                    .Select(s => new SelectListItem
                    {
                        Text = s.Nombres,
                        Value = s.Id.ToString()
                    }).ToList();
                }
            }

            switch (BasePlataforma)
            {
                case Enums.Idiomas.es_ES:
                    Model.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.es_ES, Value = s.Id.ToString() }).ToList());
                    break;
                case Enums.Idiomas.en_US:
                    Model.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.en_US, Value = s.Id.ToString() }).ToList());
                    break;
                case Enums.Idiomas.pt_BR:
                    Model.Idiomas.AddRange(Lenguaje.Select(s => new SelectListItem { Text = s.pt_BR, Value = s.Id.ToString() }).ToList());
                    break;
            }

            Model.Paises.AddRange(Paises.Select(s => new SelectListItem
            {
                Text = BasePlataforma == Enums.Idiomas.es_ES ? s.es_ES : BasePlataforma == Enums.Idiomas.en_US ? s.es_ES : s.pt_BR,
                Value = s.Valor.ToString()
            }).ToList());

            ViewBag.Asignar = "Aliados";
            if (Usuario.RolId == (int)Enums.UserRol.Administrador || Usuario.RolId == (int)Enums.UserRol.Aliado)
            {
                ViewBag.Asignar = "Encuestados";
            }

            ViewBag.Disable = "disabled";
            return View(Model);
        }

        [HttpPost]
        [Route("Busqueda")]
        public ActionResult Busqueda(ViewPollAssign Data)
        {
            ViewLogin Usuario = HttpContext.Session["login"] as ViewLogin;
            if (Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            Enums.Idiomas BasePlataforma = (Enums.Idiomas)HttpContext.Session["BaseIdioma"];
            SelectListItem Seleccione = new SelectListItem { Text = "Seleccione...", Value = "0" };

            List<MaestrasDetalle> Paises = new List<MaestrasDetalle>();
            Data.Paises = new List<SelectListItem> { Seleccione };

            List<MaestrasDetalle> Lenguaje = new List<MaestrasDetalle>();
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                Lenguaje = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Idioma")).MaestrasDetalle.ToList();
                Paises = db.Maestras.FirstOrDefault(m => m.es_ES.Equals("Pais")).MaestrasDetalle.ToList();
            }

            switch (BasePlataforma)
            {
                case Enums.Idiomas.es_ES:
                    Data.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.es_ES, Value = s.Valor, Selected = (Data.Modelo.Idioma.ToString() == s.Valor) }).ToList();
                    break;
                case Enums.Idiomas.en_US:
                    Data.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.en_US, Value = s.Valor, Selected = (Data.Modelo.Idioma.ToString() == s.Valor) }).ToList();
                    break;
                case Enums.Idiomas.pt_BR:
                    Data.Idiomas = Lenguaje.Select(s => new SelectListItem { Text = s.pt_BR, Value = s.Valor, Selected = (Data.Modelo.Idioma.ToString() == s.Valor) }).ToList();
                    break;
            }

            Data.Paises.AddRange(Paises.Select(p => new SelectListItem
            {
                Text = BasePlataforma == Enums.Idiomas.es_ES ? p.es_ES : BasePlataforma == Enums.Idiomas.en_US ? p.es_ES : p.pt_BR,
                Value = p.Valor.ToString(),
                Selected = (p.Valor == Data.Modelo.CodigoPais.ToString())
            }).ToList());

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                if (Usuario.RolId == (int)Enums.UserRol.Administrador)
                {
                    Data.Clientes = db.Usuario.Where(u => u.RolId == (int)Enums.UserRol.Cliente)
                    .Select(s => new SelectListItem
                    {
                        Text = s.Nombres,
                        Value = s.Id.ToString()
                    }).ToList();
                }
                else
                {
                    Data.Clientes = new List<SelectListItem>();
                }
            }

            using (ClsProcedures procedures = new ClsProcedures())
            {
                Data.Usuarios = procedures.UsuariosPorRol((int)Enums.UserRol.Encuestado, Data.Modelo.ClienteId, null, (int)BasePlataforma, null);
            }
            ViewBag.Disable = string.Empty;

            return View("BuscarUsuarios", Data);
        }

        [HttpPost]
        public JsonResult BuscarAliados(Guid ClienteId)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                List<SelectListItem> Result = db.Usuario.Where(u => u.ClienteId == ClienteId).Select(s => new SelectListItem { Text = s.Nombres, Value = s.Id.ToString() }).ToList();
                return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        [Route("AsignarUsuarios")]
        public ActionResult AsignarUsuarios(ViewPollAssign Data)
        {
            if (HttpContext.Session["login"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            List<ViewUser> Usuarios = JsonConvert.DeserializeObject<List<ViewUser>>(Data.SelectUser);
            List<UsuariosPorEncuenta> _Usuarios = Usuarios.Where(u => u.Estado == true).
                                                Select(s => new UsuariosPorEncuenta
                                                {
                                                    EncuestaId = (Guid)Data.DataEncuesta.id,
                                                    Id = Guid.NewGuid(),                                                    
                                                    UsuarioId = (Guid)s.Id,
                                                }).ToList();

            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                db.UsuariosPorEncuenta.AddRange(_Usuarios);
                db.SaveChanges();
            }

            return RedirectToRoute("Home");
        }

        [HttpPost]
        [Route("DesAsignarUsuarios")]
        public ActionResult DesAsignarUsuarios(UsuariosPorEncuenta Modelo)
        {
            using (ModeloEncuesta db = new ModeloEncuesta())
            {
                UsuariosPorEncuenta usuario = db.UsuariosPorEncuenta.Find(Modelo.Id);
                db.UsuariosPorEncuenta.Remove(usuario);
                db.SaveChanges();

                return RedirectToRoute("Home");
            }
        }

        [HttpPost]
        [Route("ListaUsuarios")]
        public JsonResult ListaUsuarios(string idEncuesta)
        {
            List<ViewUser> Result = new List<ViewUser>();
            Guid EncuestaId = new Guid(idEncuesta);

            using (ClsProcedures procedures = new ClsProcedures())
            {
                Result = procedures.UsuariosPorRol(null, null, null, null, EncuestaId);
            }

            return new JsonResult { Data = Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}