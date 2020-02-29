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
    public class GlCategoryController : Controller
    {
        GlCategoryRepo glCatRepo = new GlCategoryRepo();
        GlCategoryLogic categLogic = new GlCategoryLogic();
        //
        // GET: /GlCategory/
        public ActionResult Index()
        {
            return View(glCatRepo.GetAll().ToList());
        }

        // GET: /GlCategory/Create
        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            GlCategory model = new GlCategory();
            return View(model);
        }

        // POST: /GlCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MainCategory,Name,Description")] GlCategory glcategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //check for unique name
                    if (!categLogic.IsUniqueName(glcategory.Name))
                    {
                        ViewBag.Msg = "Please select another name";
                        return View();
                    }
                    glCatRepo.Insert(glcategory);
                    return RedirectToAction("Create", new { message = "Category created successfully" });
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }

            ViewBag.Msg = "Please enter correct data";
            return View(glcategory);
        }

        // GET: /GlCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlCategory glcategory = glCatRepo.GetById((int)id);
            if (glcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.Msg = "";
            return View(glcategory);
        }

        // POST: /GlCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description")] GlCategory glcategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //check for unique name
                    string originalName = glCatRepo.GetById(glcategory.ID).Name;
                    if (!glcategory.Name.ToLower().Equals(originalName.ToLower()))  //glCategory name has been changed during editting, so check if the new name doesnt exist already
                    {
                        if (!categLogic.IsUniqueName(glcategory.Name))
                        {
                            ViewBag.Msg = "Please select another name";
                            return View();
                        }
                    }

                    glCatRepo.Update(glcategory);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.Msg = "Please enter correct data";
            return View(glcategory);
        }
    }
}