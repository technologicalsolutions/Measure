using Measure.ViewModels.Error;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Measure.Utilidades
{
    public class ClsUtilities : IDisposable
    {
        private bool disposing = false;

        #region Utilidades
        /// <summary>
        /// Utilidadad para encriptar el contenido
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="Encrypt">True=encriptar/false=desencriptar</param>
        /// <returns></returns>
        public string Cifrado(string texto, bool Encrypt)
        {
            string MyKey = "3nCu35t45B^6X5bS";
            string Encriptar = string.Empty;
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            des.Key = hashmd5.ComputeHash((new UnicodeEncoding()).GetBytes(MyKey));
            des.Mode = CipherMode.ECB;
            if (Encrypt)
            {
                ICryptoTransform encrypt = des.CreateEncryptor();
                byte[] buff = Encoding.ASCII.GetBytes(texto);
                Encriptar = Convert.ToBase64String(encrypt.TransformFinalBlock(buff, 0, buff.Length));
            }
            else
            {
                ICryptoTransform desencrypta = des.CreateDecryptor();
                byte[] buff = Convert.FromBase64String(texto);
                Encriptar = Encoding.ASCII.GetString(desencrypta.TransformFinalBlock(buff, 0, buff.Length));
            }
            return Encriptar;
        }

        public ViewCatchError CatchException(DbEntityValidationException Error)
        {
            ViewCatchError Result = new ViewCatchError();
            Result.Error = true;
            Result.Mensajes = new List<string>();
            foreach (var eve in Error.EntityValidationErrors)
            {
                Result.Entidad = string.Format("La entidad de tipo \"{0}\" en el estado \"{1}\" tiene los siguientes errores de validación:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    Result.Mensajes.Add(string.Format("Propiedad: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                }
            }
            return Result;
        }

        #endregion

        #region Cache
        public List<string> Sesiones()
        {
            List<string> Sesiones = new List<string>();
            foreach (var item in HttpContext.Current.Session)
            {
                Sesiones.Add(item.ToString());
            }
            return Sesiones;
        }

        public void CrearCache<T>(string Nombre, T Datos)
        {
            List<string> _Sesiones = Sesiones();
            if (_Sesiones.Contains(Nombre))
            {
                if (Datos != null)
                {
                    HttpContext.Current.Session[Nombre] = Datos;
                }
            }
            else
            {
                HttpContext.Current.Session.Add(Nombre, Datos);
                HttpContext.Current.Session.Timeout = 60;
            }
        }

        public void EliminarUnicoCache(string cache)
        {
            HttpContext.Current.Session.Remove(cache);
        }
        #endregion

        public void Dispose()
        {
            if (disposing)
            {
                return;
            }
            disposing = true;
        }
    }
}