using Microsoft.AspNetCore.Mvc;

namespace AGSpatialDataCheck.Web.Base
{
    public class BaseViewComponent<T> : ViewComponent
    {
        public virtual IViewComponentResult InvokeAsync(T arguments)
        {
            return View(arguments);
        }
    }
}
