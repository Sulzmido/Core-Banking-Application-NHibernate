using MyCBA.Core.Models;
using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    [RestrictToAdmin]
    public class TellerManagementController : Controller
    {
        TellerMgtRepository tmRepo = new TellerMgtRepository();
        GlAccountRepository glRepo = new GlAccountRepository();
        UserRepository userRepo = new UserRepository();
        //
        // GET: /TellerManagement/
        public ActionResult Index()
        {
            return View(tmRepo.AllTellers());
        }

        public ActionResult AssignTillToUser()
        {
            try
            {
                ViewBag.TillId = new SelectList(tmRepo.TillsWithoutTeller(), "ID", "AccountName");
                ViewBag.UserId = new SelectList(tmRepo.TellersWithoutTill(), "ID", "Username");
                return View();
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTillToUser(TillToUser model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TillToUser tillToUser = new TillToUser() { UserId = model.UserId, TillId = model.TillId };
                    tmRepo.Insert(tillToUser);

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }

            //ViewBag.Msg = "Please enter correct data";
            ViewBag.TillId = new SelectList(tmRepo.TillsWithoutTeller(), "ID", "AccountName", model.TillId);
            ViewBag.UserId = new SelectList(tmRepo.TellersWithoutTill(), "ID", "Username", model.UserId);
            return View(model);
        }
    }
}