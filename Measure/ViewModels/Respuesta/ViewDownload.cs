using System.Web.Mvc;

namespace Measure.ViewModels.Respuesta
{
    public class ViewDownload
    {
        [AllowHtml]
        public string Contenido { get; set; }
        public string DataEncuesta { get; set; }
        public string DataUser { get; set; }
        public bool Mac { get; set; }
        public bool Report { get; set; }
    }
}