using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    [RestrictToLoggedIn]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.CustomersCount = new CustomerRepository().GetCount();
            ViewBag.UsersCount = new UserRepository().GetAllWithEmailConfirmed().Count;
            ViewBag.CustomerAccountCountsCount = new CustomerAccountRepository().GetCount();
            ViewBag.GlAccountCount = new GlAccountRepository().GetCount();
            ViewBag.GlCategoryCount = new GlCategoryRepo().GetCount();
            ViewBag.TellerPostingCount = new TellerPostingRepository().GetCount();
            ViewBag.GlPostingCount = new GlPostingRepository().GetCount();
            return View();
        }

    }
}