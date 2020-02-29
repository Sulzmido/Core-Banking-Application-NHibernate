using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.CustomAttribute
{
    public class RestrictToAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["roleId"] == null)
            {
                filterContext.Result = new RedirectResult("/usermanager/login");
            }

            else if ((int)HttpContext.Current.Session["roleId"] != 1)
            {
                HttpContext.Current.Session["actionRestrictionMsg"] = "You are not authorized to perform this action. Please login";
                filterContext.Result = new RedirectResult("/usermanager/actionrestrict");
                //filterContext.Result = new RedirectResult("/usermanager/login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}