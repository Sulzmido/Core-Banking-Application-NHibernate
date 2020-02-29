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
    public class BranchController : Controller
    {
        BranchRepository branchRepo;
        public BranchController()
        {
            branchRepo = new BranchRepository();
        }
        //
        // GET: /Branch/
        public ActionResult Index()
        {

            return View(branchRepo.GetAll());
        }

        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            var model = new Branch();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!(String.IsNullOrEmpty(branch.Name) || String.IsNullOrEmpty(branch.Address)))
                    {
                        //validate branch name and sort code
                        BranchLogic branchLogic = new BranchLogic();
                        if (branchLogic.IsBranchNameExists(branch.Name))
                        {
                            ViewBag.Msg = "Branch name must be unique";
                            return View();
                        }

                        branch.Status = BranchStatus.Closed;
                        branch.SortCode = branchLogic.GetSortCode();
                        branchRepo.Insert(branch);
                        return RedirectToAction("Create", new { message = "Successfully added Branch!" });
                    }
                    ViewBag.Msg = "Please enter branch name and address";
                    return View();
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.Msg = "Please enter a valid name and address";
            return View();
        }

        public ActionResult EditBranch(int? id)
        {
            ViewBag.Msg = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = branchRepo.GetById((int)id);// = db.Customers.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBranch([Bind(Include = "ID,Name,Address,SortCode")] Branch branch)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(String.IsNullOrEmpty(branch.Name) && String.IsNullOrEmpty(branch.Address)))
                    {
                        branchRepo.Update(branch);
                        ViewBag.Msg = "Updated";
                        return View();
                    }
                }
                ViewBag.Msg = "Please enter correct branch name and address";
                return View();
            }
            catch (Exception ex)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }

        public ActionResult OpenOrCloseBranch(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }
                var branch = branchRepo.GetById((int)id);
                if (branch == null)
                {
                    return HttpNotFound();
                }
                if (branch.Status == BranchStatus.Closed)
                {
                    branch.Status = BranchStatus.Open;
                }
                else
                {
                    branch.Status = BranchStatus.Closed;
                }
                branchRepo.Update(branch);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
    }
}