using MyCBA.Core.Models;
using MyCBA.Core.ViewModels.FinancialReportViewModel;
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
    public class FinancialReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TrialBalance(string date1, string date2)
        {
            try
            {
                TransactionRepository tRepo = new TransactionRepository();
                var transactions = tRepo.GetAll();

                if (!(String.IsNullOrEmpty(date1) || String.IsNullOrEmpty(date2)))
                {
                    transactions = tRepo.GetTrialBalanceTransactions(Convert.ToDateTime(date1), Convert.ToDateTime(date2));
                }
                transactions = transactions.OrderBy(t => t.MainCategory).ToList();

                var viewModel = new List<TrialBalanceViewModel>();
                decimal totalDebit = 0, totalCredit = 0;
                foreach (var transaction in transactions)
                {
                    var model = viewModel.FirstOrDefault(i => i.AccountName.ToLower().Equals((transaction.AccountName.ToLower())));
                    if (model == null)   //add new
                    {
                        model = new TrialBalanceViewModel();
                        model.AccountName = transaction.AccountName;
                        model.SubCategory = transaction.SubCategory;
                        model.MainCategory = transaction.MainCategory.ToString();
                        model.TotalCredit = transaction.TransactionType == TransactionType.Credit ? transaction.Amount : 0;
                        model.TotalDebit = transaction.TransactionType == TransactionType.Debit ? transaction.Amount : 0;
                        viewModel.Add(model);

                        totalCredit += model.TotalCredit;
                        totalDebit += model.TotalDebit;

                    }
                    else    //continue with the item
                    {
                        decimal amt = transaction.Amount;
                        if (transaction.TransactionType == TransactionType.Credit)
                        {
                            model.TotalCredit += amt;
                            totalCredit += amt;
                        }
                        else if (transaction.TransactionType == TransactionType.Debit)
                        {
                            model.TotalDebit += amt;
                            totalDebit += amt;
                        }
                    }
                }//end foreach

                ViewBag.TotalCredit = totalCredit;
                ViewBag.TotalDebit = totalDebit;
                return View(viewModel);
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }

        public ActionResult BalanceSheet()
        {
            try
            {
                BalanceSheetRepository brepo = new BalanceSheetRepository();
                //get all assets
                var assets = brepo.GetAssetAccounts();
                ViewBag.Assets = assets;
                ViewBag.AssetSum = assets.Sum(a => a.AccountBalance);
                //get all capitals
                var capitals = brepo.GetCapitalAccounts();
                ViewBag.Capitals = capitals;
                ViewBag.CapitalSum = capitals.Sum(c => c.AccountBalance);
                //get all liablilities
                var liabilities = brepo.GetLiabilityAccounts();
                ViewBag.Liability = liabilities;
                ViewBag.LiabilitySum = liabilities.Sum(l => l.Amount);
                return View();
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
        public ActionResult ProfitAndLoss(string date1, string date2)
        {
            try
            {
                ProfitAndLossRepository pRepo = new ProfitAndLossRepository();
                var entries = pRepo.GetEntries();
                ViewBag.TableTitle = "as at " + new ConfigurationRepository().GetFirst().FinancialDate.ToString("D");
                if (!(String.IsNullOrEmpty(date1) || (String.IsNullOrEmpty(date2))))
                {
                    entries = pRepo.GetEntries(Convert.ToDateTime(date1), Convert.ToDateTime(date2));
                    ViewBag.TableTitle = "Between " + Convert.ToDateTime(date1).ToString("D") + " and " + Convert.ToDateTime(date2).ToString("D");
                }
                //entries = entries.OrderBy(e => e.EntryType).ToList();
                var sortedEntries = new List<ExpenseIncomeEntry>();
                foreach (var entry in entries)
                {
                    var item = sortedEntries.FirstOrDefault(s => s.AccountName.ToUpper().Equals(entry.AccountName.ToUpper()));
                    if (item == null)
                    {
                        item = entry;
                        sortedEntries.Add(item);
                    }
                    else    //for item(s) that occur(s) twice, amt spent or earned = difference of the balances for the two occurences (days)
                    {
                        item.Amount -= entry.Amount;         //getting the difference in the account balances within the specified dates
                    }
                }
                ViewBag.SumOfIncome = sortedEntries.Where(en => en.EntryType == PandLType.Income).Sum(e => e.Amount);
                ViewBag.SumOfExpense = sortedEntries.Where(en => en.EntryType == PandLType.Expenses).Sum(e => e.Amount);
                ViewBag.Profit = (decimal)ViewBag.SumOfIncome - (decimal)ViewBag.SumOfExpense;

                return View(sortedEntries);
            }
            catch (Exception)
            {
                //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                return PartialView("Error");
            }
        }
    }
}

