using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace AGSpatialDataCheck.Web.Controllers
{
    public class RuleController : BaseController
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

        public IActionResult ListSelect()
        {
            return View();
        }
}
}