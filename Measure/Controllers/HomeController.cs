using System.Web.Mvc;

namespace Measure.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.Session["login"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }

    }
}