using Microsoft.AspNetCore.Mvc;
using ServiceCenter.Core;

namespace AGSpatialDataCheck.Web.Controllers
{
    public class UEditorController : Controller
    {
        public IActionResult Index()
        {
            var host = WebRequestHelper.GetHost(Toolset.HttpContext);
            ViewBag.Host=host;
            return View();
        }
    }
}
