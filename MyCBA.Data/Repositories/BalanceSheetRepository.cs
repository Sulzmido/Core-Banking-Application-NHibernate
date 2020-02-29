using MyCBA.Core.Models;
using MyCBA.Core.ViewModels.FinancialReportViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Data.Repositories
{
    public class BalanceSheetRepository
    {
        GlAccountRepository glactRepo = new GlAccountRepository();
        public List<GlAccount> GetAssetAccounts()
        {
            var allAssets = glactRepo.GetByMainCategory(MainGlCategory.Asset);// db.GlAccounts.Where(g => g.GlCategory.MainCategory == Core.Models.MainGlCategory.Asset).ToList();

            GlAccount loanAsset = new Core.Models.GlAccount();
            loanAsset.AccountName = "Loan Accounts";
            var loanAccounts = new CustomerAccountRepository().GetByType(AccountType.Loan);
            foreach (var act in loanAccounts)
            {
                loanAsset.AccountBalance += act.AccountBalance;
            }
            allAssets.Add(loanAsset);
            return allAssets;
        }
        public List<GlAccount> GetCapitalAccounts()
        {
            var allCapitals = glactRepo.GetByMainCategory(MainGlCategory.Capital);
            //adding the "Reserves" capitals--> Profit or loss expressed as (Income - Expense)
            GlAccount reserveCapital = new Core.Models.GlAccount();
            reserveCapital.AccountName = "Reserves";
            decimal incomeSum = glactRepo.GetByMainCategory(MainGlCategory.Income).Sum(a => a.AccountBalance);
            decimal expenseSum = glactRepo.GetByMainCategory(MainGlCategory.Expenses).Sum(a => a.AccountBalance);
            reserveCapital.AccountBalance = incomeSum - expenseSum;
            allCapitals.Add(reserveCapital);

            return allCapitals;
        }
        public List<LiabilityViewModel> GetLiabilityAccounts()
        {
            var liability = glactRepo.GetByMainCategory(MainGlCategory.Liability);

            var allLiabilityAccounts = new List<LiabilityViewModel>();

            foreach (var account in liability)
            {
                var model = new LiabilityViewModel();
                model.AccountName = account.AccountName;
                model.Amount = account.AccountBalance;

                allLiabilityAccounts.Add(model);

            }
            //adding customer's savings and loan accounts since they are liabilities to the bank           
            var savingsAccounts = new CustomerAccountRepository().GetByType(AccountType.Savings);
            var savingsLiability = new LiabilityViewModel();
            savingsLiability.AccountName = "Savings Accounts";
            savingsLiability.Amount = savingsAccounts != null ? savingsAccounts.Sum(a => a.AccountBalance) : 0;

            var currentAccounts = new CustomerAccountRepository().GetByType(AccountType.Current);
            var currentLiability = new LiabilityViewModel();
            currentLiability.AccountName = "Current Accounts";
            currentLiability.Amount = currentAccounts != null ? currentAccounts.Sum(a => a.AccountBalance) : 0;

            allLiabilityAccounts.Add(savingsLiability);
            allLiabilityAccounts.Add(currentLiability);
            return allLiabilityAccounts;
        }//end method
    }
}
