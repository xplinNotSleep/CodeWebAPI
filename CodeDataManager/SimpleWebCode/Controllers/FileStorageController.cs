using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
namespace AGSpatialDataCheck.Web.Controllers
{
    public class FileStorageController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

		public IActionResult Detail()
        {
            return View();
        }
}
}