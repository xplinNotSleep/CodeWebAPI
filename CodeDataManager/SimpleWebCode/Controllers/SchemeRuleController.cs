using AGSpatialDataCheck.Web.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace AGSpatialDataCheck.Web.Controllers
{
    public class SchemeRuleController : BaseController
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