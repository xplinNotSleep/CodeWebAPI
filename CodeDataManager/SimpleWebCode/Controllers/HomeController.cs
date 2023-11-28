using Microsoft.AspNetCore.Mvc;

namespace AGSpatialDataCheck.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult main()
        {
            return View();
        }
    }
}
