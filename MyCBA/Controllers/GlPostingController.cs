using MyCBA.Core.Models;
using MyCBA.Core.ViewModels.GlPostingViewModels;
using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using MyCBA.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    [RestrictToAdmin]
    public class GlPostingController : Controller
    {
        GlPostingRepository glPostRepo = new GlPostingRepository();
        GlAccountRepository glActRepo = new GlAccountRepository();
        BusinessLogic busLogic = new BusinessLogic();
        FinancialReportLogic frLogic = new FinancialReportLogic();
        //
        // GET: /GlPosting/
        public ActionResult Index()
        {
            return View(glPostRepo.GetAll());
        }

        //GET
        public ActionResult PostTransaction()
        {
            EodLogic logic = new EodLogic();
            if (logic.isBusinessClosed())
            {
                return PartialView("_Closed");
            }
            ViewBag.ErrorMessage = "";
            ViewBag.CrGlAccount_Id = new SelectList(glActRepo.GetAll(), "ID", "AccountName"); ;
            ViewBag.DrGlAccount_Id = new SelectList(glActRepo.GetAll(), "ID", "AccountName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostTransaction(CreateGlPostViewModel model)
        {
            EodLogic logic = new EodLogic();
            if (logic.isBusinessClosed())
            {
                return PartialView("_Closed");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.DrGlAccount_Id == model.CrGlAccount_Id)
                    {
                        return PartialView("_IncorrectData");
                    }

                    if (model.CreditAmount == model.DebitAmount && model.CreditAmount > 0)    //double checking
                    {
                        var drAct = glActRepo.GetById(model.DrGlAccount_Id);
                        var crAct = glActRepo.GetById(model.CrGlAccount_Id);
                        //check for sufficient balance on Vault and Tills
                        if (crAct.AccountName.ToLower().Contains("till") || crAct.AccountName.ToLower().Contains("vault"))
                        {
                            if (crAct.AccountBalance < model.CreditAmount)
                            {
                                return PartialView("_InsufficientBalance");
                            }
                        }


                        var user = getLoggedInUser();
                        if (user == null || user.Role.ID != 1)  //admin with a role ID of 1
                        {
                            return RedirectToAction("Login", "UserManager", new { returnUrl = "/glposting/posttransaction" });
                        }

                        decimal amt = model.CreditAmount;
                        GlPosting glPosting = new GlPosting { CreditAmount = amt, DebitAmount = amt, Date = DateTime.Now, CrGlAccount = crAct, DrGlAccount = drAct, Naration = model.Naration, PostInitiator = user };
                        busLogic.CreditGl(crAct, amt);
                        busLogic.DebitGl(drAct, amt);

                        glPostRepo.Insert(glPosting);
                        glActRepo.Update(crAct);
                        glActRepo.Update(drAct);
                        frLogic.CreateTransaction(drAct, amt, TransactionType.Debit);
                        frLogic.CreateTransaction(crAct, amt, TransactionType.Credit);
                        return PartialView("_SuccessPost");

                    }
                }
                catch (Exception)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }//end if
            ViewBag.DrGlAccount_Id = new SelectList(glActRepo.GetAll(), "ID", "AccountName", model.DrGlAccount_Id);
            ViewBag.CrGlAccount_Id = new SelectList(glActRepo.GetAll(), "ID", "AccountName", model.CrGlAccount_Id);
            return PartialView("_IncorrectData");
        }

        public User getLoggedInUser()
        {
            if (String.IsNullOrEmpty((String)Session["username"]))
            {
                return null;
            }
            return new UserRepository().GetByUsername((String)Session["username"]);
        }
    }
}