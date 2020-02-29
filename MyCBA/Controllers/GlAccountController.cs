using MyCBA.Core.Models;
using MyCBA.Core.ViewModels.GlViewModels;
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
    public class GlAccountController : Controller
    {
        GlAccountRepository glactRepo = new GlAccountRepository();
        GlCategoryRepo glCatRepo = new GlCategoryRepo();
        BranchRepository branchRepo = new BranchRepository();
        GlAccountLogic gllogic = new GlAccountLogic();
        //
        // GET: /GlAccount/
        public ActionResult Index()
        {
            var glaccounts = glactRepo.GetAll();
            return View(glaccounts.ToList());
        }

        // GET: /GlAccount/Create
        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            ViewBag.GlCategoryId = new SelectList(glCatRepo.GetAll(), "ID", "Name");
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name");
            var model = new AddGlActViewModel();
            return View(model);
        }

        // POST: /GlAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddGlActViewModel model)
        {
            ViewBag.GlCategoryId = new SelectList(glCatRepo.GetAll(), "ID", "Name", model.GlCategoryId);
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);

            if (ModelState.IsValid)
            {
                try
                {
                    var category = glCatRepo.GetById(model.GlCategoryId);
                    var branch = branchRepo.GetById(model.BranchId);
                    MainGlCategory mainCategory = (MainGlCategory)((int)category.MainCategory);

                    //if is unique account name
                    if (!gllogic.IsUniqueName(model.AccountName))
                    {
                        ViewBag.Msg = "Account name must be unique";
                        return View(model);
                    }

                    GlAccount glAccount = new GlAccount() { AccountName = model.AccountName, GlCategory = category, Branch = branch, CodeNumber = gllogic.GenerateGLAccountNumber(mainCategory) };
                    glactRepo.Insert(glAccount);
                    //ViewBag.Msg = "successfully added account";

                    return RedirectToAction("Create", new { message = "successfully added account" });
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }

            ViewBag.Msg = "Please enter correct data";
            return View(model);
        }

        // GET: /GlAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlAccount glaccount = glactRepo.GetById((int)id);
            if (glaccount == null)
            {
                return HttpNotFound();
            }
            var model = new EditGlAccountViewModel();
            model.Id = glaccount.ID;
            model.AccountName = glaccount.AccountName;
            model.BranchId = glaccount.Branch.ID;

            ViewBag.Msg = "";
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", glaccount.Branch.ID);
            return View(model);
        }

        // POST: /GlAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountName,BranchId")] EditGlAccountViewModel model, string BranchId)
        {
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);

            if (ModelState.IsValid)
            {
                try
                {
                    var glaccount = glactRepo.GetById(model.Id);
                    //check for unique name
                    string originalName = glaccount.AccountName;
                    if (!model.AccountName.ToLower().Equals(originalName.ToLower()))  //glCategory name has been changed during editting, so check if the new name doesnt exist already
                    {
                        if (!gllogic.IsUniqueName(model.AccountName))
                        {
                            ViewBag.Msg = "Please select another name";
                            return View();
                        }
                    }
                    var branch = branchRepo.GetById(model.BranchId);
                    if (branch == null)
                    {
                        ViewBag.Msg = "Please select a branch";
                        return View(model);
                    }
                    glaccount.AccountName = model.AccountName;
                    glaccount.Branch = branch;

                    glactRepo.Update(glaccount);
                    ViewBag.Msg = "Data updatded successfully!";
                    return View();
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.Msg = "Please enter correct data";
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlAccount glaccount = glactRepo.GetById((int)id);
            if (glaccount == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", glaccount);
        }
    }
}