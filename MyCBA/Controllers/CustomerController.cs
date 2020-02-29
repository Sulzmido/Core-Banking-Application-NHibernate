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
    public class CustomerController : Controller
    {
        CustomerRepository custRepo = new CustomerRepository();
        CustomerLogic custLogic = new CustomerLogic();
        //
        // GET: /Customer/
        public ActionResult Index()
        {
            var customers = custRepo.GetAll();
            return View(customers);
        }
        // GET: /Customer/Create
        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            var model = new Customer();
            return View(model);
        }

        // POST: /Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustId,FullName,Address,Email,PhoneNumber,Gender")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //generate customer ID
                    customer.CustId = custLogic.GenerateCustomerId();
                    custRepo.Insert(customer);
                    return RedirectToAction("Create", new { message = "Data saved successfully" });       //to remove all data of the saved customer from the page                
                }
                ViewBag.Msg = "An Error occured! please try again";
                return View(customer);
            }
            catch (Exception ex)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = custRepo.GetById((int)id);// db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", customer);
        }

        // GET: /Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Msg = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = custRepo.GetById((int)id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustId,Status,FullName,Address,Email,PhoneNumber,Gender")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    custRepo.Update(customer);
                    ViewBag.Msg = "Customer data updated sucesfully";
                    return View();
                }
                ViewBag.Msg = "Please enter corect data";
                return View(customer);
            }
            catch (Exception ex)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
    }
}