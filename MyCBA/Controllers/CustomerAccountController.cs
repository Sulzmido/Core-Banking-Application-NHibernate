using MyCBA.Core.Models;
using MyCBA.Core.ViewModels.CustomerAccountViewModels;
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
    public class CustomerAccountController : Controller
    {
        CustomerAccountRepository actRepo = new CustomerAccountRepository();
        BranchRepository branchRepo = new BranchRepository();
        CustomerRepository custRepo = new CustomerRepository();
        ConfigurationRepository configRepo = new ConfigurationRepository();
        CustomerAccountLogic logic = new CustomerAccountLogic();
        BusinessLogic busLogic = new BusinessLogic();
        //
        // GET: /CustomerAccount/
        public ActionResult Index()
        {
            return View(actRepo.GetAll());
        }
        public ActionResult AddAccount()
        {
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name");
            ViewBag.CustomerId = new SelectList(custRepo.GetAll(), "ID", "FullName");

            var config = configRepo.GetFirst();
            ViewBag.LoanInterestRate = config.LoanDebitInterestRate.ToString();
            ViewBag.Msg = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccount(CustomerAccount model, string BranchId, string InterestRate, string NumberOfYears)
        {
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = custRepo.GetById(model.CustId);
                    if (customer == null)
                    {
                        ViewBag.Msg = "Incorrect customer Id";
                        return View();
                    }

                    var act = new CustomerAccount();
                    act.AccountName = model.AccountName;
                    act.AccountType = model.AccountType;
                    act.Customer = customer;
                    act.AccountNumber = logic.GenerateCustomerAccountNumber(model.AccountType, customer.ID);
                    act.DateCreated = DateTime.Now;

                    int bid = 0;
                    if (!int.TryParse(BranchId, out bid))
                    {
                        ViewBag.Msg = "Branch cannot be null";
                        return View();
                    }
                    var branch = branchRepo.GetById(bid); //db.Branches.Where(b => b.BranchId == bid).SingleOrDefault();
                    if (branch == null)
                    {
                        ViewBag.Msg = "Branch cannot be null";
                        return View();
                    }
                    act.Branch = branch;

                    if (!(model.AccountType == AccountType.Loan))   //for savings and current
                    {
                        actRepo.Insert(act);
                        ViewBag.Msg = "Account successfully created";
                        return RedirectToAction("Index");
                    }
                    else   //   FOR LOAN ACCOUNTS
                    {
                        if (model.LoanAmount < 1000 || model.LoanAmount >= decimal.MaxValue)
                        {
                            ViewBag.Msg = "Loan amount must be between #1,000 and a maximum reasonable amount";
                            return View(model);
                        }
                        act.LoanAmount = model.LoanAmount;
                        act.TermsOfLoan = model.TermsOfLoan;
                        var servAct = actRepo.GetByAccountNumber(model.ServicingAccountId);
                        if (servAct == null || servAct.AccountType == AccountType.Loan)
                        {
                            ViewBag.Msg = "Invalid account selected";
                            return View(model);
                        }
                        act.ServicingAccount = servAct;

                        double interestRate = 0;
                        double nyears = 0;

                        if (!(double.TryParse(InterestRate, out interestRate) && double.TryParse(NumberOfYears, out nyears)))
                        {
                            ViewBag.Msg = "Number of years or Interest rate value is incorrect";
                            return View(model);
                        }
                        act.LoanInterestRatePerMonth = Convert.ToDecimal(InterestRate);
                        if (!(interestRate > 0 && nyears > 0 && model.LoanAmount > 0))
                        {
                            ViewBag.Msg = "Please enter positive values";
                            return View(model);
                        }
                        switch (act.TermsOfLoan)
                        {
                            case TermsOfLoan.Fixed:
                                logic.ComputeFixedRepayment(act, nyears, interestRate);
                                break;
                            case TermsOfLoan.Reducing:
                                logic.ComputeReducingRepayment(act, nyears, interestRate);
                                break;
                            default:
                                break;
                        }

                        busLogic.DebitCustomerAccount(act, (decimal)act.LoanAmount);        //debit the loan act (An Asset to the Bank)
                        busLogic.CreditCustomerAccount(act.ServicingAccount, (decimal)act.LoanAmount);  //credit the loan's servicing account
                        new FinancialReportLogic().CreateTransaction(act, act.LoanAmount, TransactionType.Debit);
                        new FinancialReportLogic().CreateTransaction(act.ServicingAccount, act.LoanAmount, TransactionType.Credit);

                        actRepo.Update(act.ServicingAccount);
                        actRepo.Insert(act);
                        ViewBag.Msg = "Account successfully created";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return View(model);
                }
            }//end if ModelState
            else
            {
                ViewBag.Msg = "Please enter correct data";
                return View(model);
            }

            // return View(model);
        }// end addAccount

        public ActionResult EditAccount(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerAccount act = actRepo.GetById((int)id);
                if (act == null)
                {
                    return HttpNotFound();
                }
                var model = new EditCustomerAccountViewModel();
                model.Id = act.ID;
                model.AccountName = act.AccountName;
                model.BranchId = act.BranchId;

                ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);
                return View(model);
            }
            catch (Exception ex)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(EditCustomerAccountViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = actRepo.GetById(model.Id);
                    account.AccountName = model.AccountName;
                    account.BranchId = model.BranchId;
                    actRepo.Update(account);

                    return RedirectToAction("Index");
                }

                ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);
                return View(model);
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }//end Edit

        public ActionResult ChangeAccountStatus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerAccount act = actRepo.GetById((int)id);
            if (act == null)
            {
                return HttpNotFound();
            }
            if (act.AccountStatus == AccountStatus.Active)
            {
                act.AccountStatus = AccountStatus.Closed;
            }
            else
            {
                act.AccountStatus = AccountStatus.Active;
            }
            actRepo.Update(act);
            return RedirectToAction("Index");
        }

        public ActionResult Details(int Id)
        {
            CustomerAccount account = new CustomerAccount();
            account = actRepo.GetById(Id);
            if (account == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return PartialView("_Details", account);
        }

        public ActionResult GetAllAccountsInJson(string searchString)
        {
            try
            {
                var CustAcctModel = new List<CustomerAccountJsonViewModel>();
                var accounts = actRepo.GetAll().ToList();
                foreach (var item in accounts)
                {
                    if (item.AccountName.ToLower().Contains(searchString.ToLower()) || item.AccountNumber.ToString().Contains(searchString))
                    {
                        CustomerAccountJsonViewModel model = new CustomerAccountJsonViewModel();
                        model.Label = item.AccountName + ": " + item.AccountNumber;
                        model.Value = item.AccountNumber.ToString();
                        CustAcctModel.Add(model);
                    }
                }
                var jact = Json(CustAcctModel);
                return jact;
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
    }
}