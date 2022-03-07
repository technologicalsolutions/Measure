using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Measure.ViewModels.Error
{
    public class ViewCatchError
    {
        public bool Error { get; set; }
        public string Entidad { get; set; }
        public List<string> Mensajes { get; set; }
    }
}