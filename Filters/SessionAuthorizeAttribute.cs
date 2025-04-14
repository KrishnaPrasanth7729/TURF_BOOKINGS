using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TURF_BOOKINGS.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("UserEmail") == null)
            {
                context.Result = new RedirectToActionResult("LogViews", "Home", null);
            }
            base.OnActionExecuting(context);
        }
    }
}



