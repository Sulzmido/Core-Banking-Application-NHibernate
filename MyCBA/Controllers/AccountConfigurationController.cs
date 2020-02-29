using MyCBA.Core.Models;
using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    [RestrictToAdmin]
    public class AccountConfigurationController : Controller
    {
        ConfigurationRepository configRepo = new ConfigurationRepository();
        GlAccountRepository glaRepo = new GlAccountRepository();
        //
        // GET: /AccountConfiguration/
        public ActionResult Index()
        {
            var accountconfiguration = configRepo.GetFirst();
            if (accountconfiguration == null)
            {
                return HttpNotFound();
            }
            return View(accountconfiguration);
        }
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AccountConfiguration accountconfiguration = configRepo.GetById((int)id);
                if (accountconfiguration == null)
                {
                    return HttpNotFound();
                }
                ViewBag.SavingsInterestExpenseGl_GlAccountId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", accountconfiguration.SavingsInterestExpenseGl != null ? accountconfiguration.SavingsInterestExpenseGl.ID : 0);
                ViewBag.SavingsInterestPayableGl_Id = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Liability), "ID", "AccountName", accountconfiguration.SavingsInterestPayableGl != null ? accountconfiguration.SavingsInterestPayableGl.ID : 0);
                ViewBag.CurrentIntExpGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", accountconfiguration.CurrentInterestExpenseGl != null ? accountconfiguration.CurrentInterestExpenseGl.ID : 0);
                ViewBag.CurrentCotIncGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Income), "ID", "AccountName", accountconfiguration.CurrentCotIncomeGl != null ? accountconfiguration.CurrentCotIncomeGl.ID : 0);
                ViewBag.LoanIntIncomeGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Income), "ID", "AccountName", accountconfiguration.LoanInterestIncomeGl != null ? accountconfiguration.LoanInterestIncomeGl.ID : 0);
                //ViewBag.LoanIntExpGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", accountconfiguration.LoanInterestExpenseGl != null ? accountconfiguration.LoanInterestExpenseGl.ID : 0);
                ViewBag.LoanInterestReceivableGl_Id = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Asset), "ID", "AccountName", accountconfiguration.LoanInterestReceivableGl != null ? accountconfiguration.LoanInterestReceivableGl.ID : 0);
                return View(accountconfiguration);
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }

        // POST: /AccountConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SavingsCreditInterestRate,SavingsMinimumBalance,CurrentCreditInterestRate,CurrentMinimumBalance,CurrentCot,LoanDebitInterestRate")] AccountConfiguration model, string SavingsInterestExpenseGl_GlAccountId, string CurrentIntExpGlId, string CurrentCotIncGlId, string LoanIntIncomeGlId,  string LoanInterestReceivableGl_Id, string SavingsInterestPayableGl_Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AccountConfiguration accountconfiguration = configRepo.GetById((int)model.ID);
                    accountconfiguration.SavingsCreditInterestRate = model.SavingsCreditInterestRate;
                    accountconfiguration.SavingsMinimumBalance = model.SavingsMinimumBalance;
                    accountconfiguration.CurrentCot = model.CurrentCot;
                    accountconfiguration.CurrentCreditInterestRate = model.CurrentCreditInterestRate;
                    accountconfiguration.CurrentMinimumBalance = model.CurrentMinimumBalance;
                    accountconfiguration.LoanDebitInterestRate = model.LoanDebitInterestRate;

                    if (!String.IsNullOrEmpty(SavingsInterestExpenseGl_GlAccountId))
                    {
                        int x = Convert.ToInt32(SavingsInterestExpenseGl_GlAccountId);
                        accountconfiguration.SavingsInterestExpenseGl = glaRepo.GetById(x);
                    }
                    if (!String.IsNullOrEmpty(SavingsInterestPayableGl_Id))
                    {
                        int x = Convert.ToInt32(SavingsInterestPayableGl_Id);
                        accountconfiguration.SavingsInterestPayableGl = glaRepo.GetById(x);
                    }
                    if (!String.IsNullOrEmpty(CurrentIntExpGlId))
                    {
                        int x = Convert.ToInt32(CurrentIntExpGlId);
                        accountconfiguration.CurrentInterestExpenseGl = glaRepo.GetById(x);
                    }
                    if (!String.IsNullOrEmpty(CurrentCotIncGlId))
                    {
                        int x = Convert.ToInt32(CurrentCotIncGlId);
                        accountconfiguration.CurrentCotIncomeGl = glaRepo.GetById(x);
                    }
                    if (!String.IsNullOrEmpty(LoanIntIncomeGlId))
                    {
                        int x = Convert.ToInt32(LoanIntIncomeGlId);
                        accountconfiguration.LoanInterestIncomeGl = glaRepo.GetById(x);
                    }
                    /*
                    if (!String.IsNullOrEmpty(LoanIntExpGlId))
                    {
                        int x = Convert.ToInt32(LoanIntExpGlId);
                        accountconfiguration.LoanInterestExpenseGl = glaRepo.GetById(x);
                    }*/
                    if (!String.IsNullOrEmpty(LoanInterestReceivableGl_Id))
                    {
                        int x = Convert.ToInt32(LoanInterestReceivableGl_Id);
                        accountconfiguration.LoanInterestReceivableGl = glaRepo.GetById(x);
                    }
                    configRepo.Update(accountconfiguration);
                    return RedirectToAction("Index");
                }
                var config = new ConfigurationRepository().GetFirst();
                ViewBag.SavingsInterestExpenseGl_GlAccountId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", config.SavingsInterestExpenseGl.ID);
                ViewBag.SavingsInterestPayableGl_Id = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Liability), "ID", "AccountName", config.SavingsInterestPayableGl.ID);
                ViewBag.CurrentIntExpGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", config.CurrentInterestExpenseGl.ID);
                ViewBag.CurrentCotIncGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Income), "ID", "AccountName", config.CurrentCotIncomeGl.ID);
                ViewBag.LoanIntIncomeGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Income), "ID", "AccountName", config.LoanInterestIncomeGl.ID);
                //ViewBag.LoanIntExpGlId = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Expenses), "ID", "AccountName", config.LoanInterestExpenseGl.ID);
                ViewBag.LoanInterestReceivableGl_Id = new SelectList(glaRepo.GetByMainCategory(MainGlCategory.Asset), "ID", "AccountName", config.LoanInterestReceivableGl.ID);
                return View(model);
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
    }
}