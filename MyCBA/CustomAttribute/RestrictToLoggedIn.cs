using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.CustomAttribute
{
    public class RestrictToLoggedIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["roleId"] == null)
            {
                filterContext.Result = new RedirectResult("/usermanager/login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}