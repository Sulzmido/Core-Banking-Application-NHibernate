using MyCBA.Core.Models;
using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using MyCBA.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    [RestrictToAdmin]
    public class RoleManagerController : Controller
    {
        RoleRepository roleRepo = new RoleRepository();
        RoleLogic roleLogic = new RoleLogic();
        //
        // GET: /RoleManager/
        public ActionResult Index()
        {
            return View(roleRepo.GetAll());
        }

        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            var model = new Role();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!(String.IsNullOrEmpty(role.Name)))
                    {
                        if (roleLogic.isRoleNameExists(role.Name))
                        {
                            ViewBag.Msg = "Role name must be unique";
                            return View();
                        }
                        roleRepo.Insert(role);
                        return RedirectToAction("Create", new { message = "Successfully added Role!" });
                    }
                    ViewBag.Msg = "Please enter role name";
                    return View();
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.Msg = "Please enter a valid name";
            return View();
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.Msg = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = roleRepo.GetById((int)id);// = db.Customers.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Role role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!(String.IsNullOrEmpty(role.Name)))
                    {
                        string originalName = roleRepo.GetById(role.ID).Name;
                        if (!role.Name.ToLower().Equals(originalName.ToLower()))
                        {
                            if (roleLogic.isRoleNameExists(role.Name))
                            {
                                ViewBag.Msg = "Role name must be unique";
                                return View();
                            }
                        }

                        roleRepo.Update(role);
                        ViewBag.Msg = "Updated";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.Msg = "Please enter correct branch name and address";
            return View();
        }
    }
}