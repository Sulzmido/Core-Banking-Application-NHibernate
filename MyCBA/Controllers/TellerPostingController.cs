using MyCBA.Core.Models;
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
    
    public class TellerPostingController : Controller
    {
        TellerPostingRepository tpRepo = new TellerPostingRepository();
        CustomerAccountRepository custActRepo = new CustomerAccountRepository();
        //
        // GET: /TellerPosting/
        public ActionResult Index()
        {
            var telPostings = tpRepo.GetAll();
            return View(telPostings);
        }

        [RestrictToTeller]
        public ActionResult Post()
        {
            return View(custActRepo.GetAll());
        }

        public ActionResult PostingDetails(int? accountId)
        {
            if (accountId == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var account = custActRepo.GetById((int)accountId);
            if (account == null)
            {
                return HttpNotFound();
            }
            TellerPosting posting = new TellerPosting();
            posting.CustomerAccount = account;

            CustomerAccount sessionAccount = GetAccountToPostTo();
            sessionAccount = account;
            Session["custAccount"] = sessionAccount;
            return PartialView("_PostDetails", posting);
        }
        private CustomerAccount GetAccountToPostTo()    //getting from session
        {
            if (Session["custAccount"] == null)
            {
                Session["custAccount"] = new CustomerAccount();
            }
            return (CustomerAccount)Session["custAccount"];
        }

        private void RemoveAccountFromSession()
        {
            Session.Remove("custAccount");
        }

        [RestrictToTeller]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostTransaction(TellerPosting model)
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
                    if (model.Amount <= 0)
                    {
                        return PartialView("_IncorrectData");
                    }

                    var loggedInTeller = getLoggedInUser();
                    if (loggedInTeller == null)
                        return PartialView("_UnauthorizedTeller");
                    var tillAct = new TellerMgtRepository().GetUserTill(loggedInTeller);
                    if (tillAct == null)
                    {
                        return PartialView("_UnauthorizedTeller");
                    }

                    var act = (CustomerAccount)Session["custAccount"];
                    var account = custActRepo.GetByAccountNumber(act.AccountNumber);

                    if (account == null)
                    {
                        ViewBag.ErrorMessage = "Invalid account selected";
                        return PartialView("_InvalidAccount");
                    }

                    model.Date = DateTime.Now;
                    TellerPosting telPosting = new TellerPosting { Amount = model.Amount, Narration = model.Narration, Date = DateTime.Now, PostingType = model.PostingType, CustomerAccount = account, PostInitiator = loggedInTeller };
                    //check for balance sufficiency upon withdrawal
                    if (model.PostingType == TellerPostingType.Withdrawal)
                    {
                        if (new CustomerAccountLogic().CustomerAccountHasSufficientBalance(account, model.Amount))
                        {
                            if (!(tillAct.AccountBalance >= model.Amount))
                            {
                                return PartialView("_TellerInsufficientBalance");
                            }
                            string result = new TellerPostingLogic().PostTeller(account, tillAct, model.Amount, model.PostingType);
                            if (!result.Equals("success"))
                            {
                                return PartialView("_UnknownError");
                            }
                            tpRepo.Insert(telPosting);
                            new CustomerAccountRepository().Update(account);
                            new GlAccountRepository().Update(tillAct);

                            new FinancialReportLogic().CreateTransaction(tillAct, model.Amount, TransactionType.Credit);
                            new FinancialReportLogic().CreateTransaction(account, model.Amount, TransactionType.Debit);

                            RemoveAccountFromSession();
                            return PartialView("_SuccessPost");

                        }
                        else    //no sufficient balance
                        {
                            ViewBag.ErrorMessage = "Insufficient balance";
                            return PartialView("_InsufficientBalance");
                        }
                    }
                    else  //deposit
                    {
                        string result = new TellerPostingLogic().PostTeller(account, tillAct, model.Amount, model.PostingType);
                        if (!result.Equals("success"))
                        {
                            return PartialView("_UnknownError");
                        }
                        tpRepo.Insert(telPosting);
                        new CustomerAccountRepository().Update(account);
                        new GlAccountRepository().Update(tillAct);

                        new FinancialReportLogic().CreateTransaction(tillAct, model.Amount, TransactionType.Debit);
                        new FinancialReportLogic().CreateTransaction(account, model.Amount, TransactionType.Credit);

                        RemoveAccountFromSession();
                        return PartialView("_SuccessPost");
                    }
                }
                catch (Exception)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }

            }
            ViewBag.CustomerAccountId = new SelectList(custActRepo.GetAll().Where(a => a.AccountType != AccountType.Loan), "ID", "AccountNumber");
            return PartialView("_IncorrectData");
        }//       

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