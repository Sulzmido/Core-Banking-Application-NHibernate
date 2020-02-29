using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class EodLogic
    {
        AccountConfiguration config = new ConfigurationRepository().GetFirst();
        ConfigurationRepository configRepo = new ConfigurationRepository();
        CustomerAccountRepository custActRepo = new CustomerAccountRepository();
        GlAccountRepository glRepo = new GlAccountRepository();
        BusinessLogic busLogic = new BusinessLogic();
        FinancialReportLogic frLogic = new FinancialReportLogic();
        DateTime today;

        public EodLogic()
        {
            today = config.FinancialDate;
        }
        int[] daysInMonth = new int[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public string RunEOD()
        {
            string output = "";
            //check if all configurations are set
            try
            {
                if (busLogic.IsConfigurationSet())
                {
                    CloseBusiness();
                    ComputeSavingsInterestAccrued();
                    ComputeCurrentInterestAccrued();
                    ComputeLoanInterestAccrued();
                    SaveDailyIncomeAndExpenseBalance();     //to calculate Profit and Loss

                    //var config = db.AccountConfiguration.First();
                    config.FinancialDate = config.FinancialDate.AddDays(1);        //increments the financial date at the EOD

                    configRepo.Update(config);           //Ensures all or none of the 5 steps above executes and gets saved                     
                    output = "Successfully Run EOD!";
                }
                else
                {
                    output = "Configurations not set!";
                }
            }
            catch (Exception)
            {
                output = "An error occured while running EOD";
            }
            return output;
        }
        public void CloseBusiness()
        {
            //var config = db.AccountConfiguration.First();
            config.IsBusinessOpen = false;
        }
        public void OpenBusiness()
        {
            //var config = db.AccountConfiguration.First();
            config.IsBusinessOpen = true;
            configRepo.Update(config);
            //db.SaveChanges();
        }
        public bool isBusinessClosed()
        {
            //var config = new ConfigurationRepository().GetFirst();
            if (config.IsBusinessOpen)
            {
                return false;
            }
            else
            {
                return true;
            }
        }//

        public void ComputeSavingsInterestAccrued()
        {
            //var config = db.AccountConfiguration.First();
            //DateTime today = DateTime.Now;
            int presentDay = today.Day;     //1 to totalDays in d month
            int presentMonth = today.Month;     //1 to 12
            int daysRemaining = 0;
            if (custActRepo.AnyAccountOfType(AccountType.Savings))
            {
                var allSavings = custActRepo.GetByType(AccountType.Savings);
                //foreach savings account
                foreach (var account in allSavings)
                {
                    //get the no of days remaining in this month
                    daysRemaining = daysInMonth[presentMonth - 1] - presentDay + 1;     //+1 because we havent computed for today
                    decimal interestRemainingForTheMonth = account.AccountBalance * (decimal)config.SavingsCreditInterestRate * daysRemaining / daysInMonth[presentMonth - 1];      //using I = PRT, where R is per month
                    //calculate today's interest and add it to the account's dailyInterestAccrued
                    decimal todaysInterest = interestRemainingForTheMonth / daysRemaining;
                    account.dailyInterestAccrued += todaysInterest;     //increments till month end. Disbursed if withdrawal limit is not exceeded

                    busLogic.DebitGl(config.SavingsInterestExpenseGl, todaysInterest);
                    busLogic.CreditGl(config.SavingsInterestPayableGl, todaysInterest);

                    custActRepo.Update(account);
                    glRepo.Update(config.SavingsInterestExpenseGl);
                    glRepo.Update(config.SavingsInterestPayableGl);

                    frLogic.CreateTransaction(config.SavingsInterestExpenseGl, todaysInterest, TransactionType.Debit);
                    frLogic.CreateTransaction(config.SavingsInterestPayableGl, todaysInterest, TransactionType.Credit);
                }//end foreach

                //monthly savings interest payment
                if (today.Day == daysInMonth[presentMonth - 1])     //MONTH END?
                {
                    bool customerIsCredited = false;
                    foreach (var account in allSavings)
                    {
                        busLogic.DebitGl(config.SavingsInterestPayableGl, account.dailyInterestAccrued);

                        //if the Withdrawal limit is not exceeded
                        if (!(account.SavingsWithdrawalCount > 3))    //assuming the withdrawal limit is 3
                        {
                            //Credit the customer ammount
                            busLogic.CreditCustomerAccount(account, account.dailyInterestAccrued);
                            customerIsCredited = true;
                        }
                        else
                        {
                            busLogic.CreditGl(config.SavingsInterestExpenseGl, account.dailyInterestAccrued);
                        }
                        account.SavingsWithdrawalCount = 0;  //re-initialize to zero for the next month
                        account.dailyInterestAccrued = 0;
                        custActRepo.Update(account);
                        glRepo.Update(config.SavingsInterestExpenseGl);
                        glRepo.Update(config.SavingsInterestPayableGl);

                        frLogic.CreateTransaction(config.SavingsInterestPayableGl, account.dailyInterestAccrued, TransactionType.Debit);
                        if (customerIsCredited)
                        {
                            frLogic.CreateTransaction(account, account.dailyInterestAccrued, TransactionType.Credit);
                        }
                        frLogic.CreateTransaction(config.SavingsInterestExpenseGl, account.dailyInterestAccrued, TransactionType.Credit);                                                
                    }
                }//end if
                configRepo.Update(config);
            }
        }//end method ComputeAllSavingsInterestAccrued

        public void ComputeCurrentInterestAccrued()
        {
            if (custActRepo.AnyAccountOfType(AccountType.Current))
            {
                //note that COT is calculated upon withdarawal and not on a daily basis
                //the accrued COT is then deducted at month end
                int presentMonth = today.Month;     //1 to 12
                //monthly COT deduction
                if (today.Day == daysInMonth[presentMonth - 1])     //MONTH END?
                {
                    var allCurrents = custActRepo.GetByType(AccountType.Current);
                    foreach (var currentAccount in allCurrents)
                    {
                        //Debit customer account
                        //currentAccount.AccountBalance -= currentAccount.dailyInterestAccrued;   //accrued COT
                        busLogic.DebitCustomerAccount(currentAccount, currentAccount.dailyInterestAccrued);
                        busLogic.CreditGl(config.CurrentCotIncomeGl, currentAccount.dailyInterestAccrued);

                        currentAccount.dailyInterestAccrued = 0;    //goes back to zero after monthly deduction
                        custActRepo.Update(currentAccount);
                        glRepo.Update(config.CurrentCotIncomeGl);

                        frLogic.CreateTransaction(currentAccount, currentAccount.dailyInterestAccrued, TransactionType.Debit);
                        frLogic.CreateTransaction(config.CurrentCotIncomeGl, currentAccount.dailyInterestAccrued, TransactionType.Credit);
                    }
                    configRepo.Update(config);
                }//end if
                //db.SaveChanges();
            }//end if    
        }//end current compute

        public void ComputeLoanInterestAccrued()
        {
            int presentMonth = today.Month;     //1 to 12
            decimal dailyInterestRepay = 0;

            if (custActRepo.AnyAccountOfType(AccountType.Loan))
            {
                var allLoans = custActRepo.GetByType(AccountType.Loan);
                //daily loan repay
                foreach (var loanAccount in allLoans)
                {
                    dailyInterestRepay = loanAccount.LoanMonthlyInterestRepay / 30;     //assume 30 days in a month
                    loanAccount.dailyInterestAccrued += dailyInterestRepay;

                    busLogic.DebitGl(config.LoanInterestReceivableGl, dailyInterestRepay);
                    busLogic.CreditGl(config.LoanInterestIncomeGl, dailyInterestRepay);

                    loanAccount.DaysCount++;  //increments the day account was created after every EOD
                    custActRepo.Update(loanAccount);
                    configRepo.Update(config);
                    glRepo.Update(config.LoanInterestReceivableGl);
                    glRepo.Update(config.LoanInterestIncomeGl);

                    frLogic.CreateTransaction(config.LoanInterestReceivableGl, dailyInterestRepay, TransactionType.Debit);
                    frLogic.CreateTransaction(config.LoanInterestIncomeGl, dailyInterestRepay, TransactionType.Credit);
                }//end foreach

                //monthly loan deduction
                foreach (var loanAccount in allLoans)
                {
                    if (loanAccount.DaysCount % 30 == 0)      //checks monthly (30 days) cycle
                    {
                        //pay back interest
                        busLogic.CreditGl(config.LoanInterestReceivableGl, loanAccount.dailyInterestAccrued);    //so the interestReceivable account balance goes back to zero                      
                        busLogic.DebitCustomerAccount(loanAccount.ServicingAccount, loanAccount.dailyInterestAccrued);

                        glRepo.Update(config.LoanInterestReceivableGl);
                        custActRepo.Update(loanAccount.ServicingAccount);

                        frLogic.CreateTransaction(config.LoanInterestReceivableGl, loanAccount.dailyInterestAccrued, TransactionType.Credit);
                        frLogic.CreateTransaction(loanAccount.ServicingAccount, loanAccount.dailyInterestAccrued, TransactionType.Debit);

                        loanAccount.dailyInterestAccrued = 0;       //returns to zero after it has been debitted

                        //pay back monthly principal
                        busLogic.CreditCustomerAccount(loanAccount, loanAccount.LoanMonthlyPrincipalRepay);
                        busLogic.DebitCustomerAccount(loanAccount.ServicingAccount, loanAccount.LoanMonthlyPrincipalRepay);

                        custActRepo.Update(loanAccount);
                        custActRepo.Update(loanAccount.ServicingAccount);

                        frLogic.CreateTransaction(loanAccount, loanAccount.LoanMonthlyPrincipalRepay, TransactionType.Credit);
                        frLogic.CreateTransaction(loanAccount.ServicingAccount, loanAccount.LoanMonthlyPrincipalRepay, TransactionType.Debit);

                        if (loanAccount.TermsOfLoan == TermsOfLoan.Reducing)        //the monthly paymment will change
                        {
                            loanAccount.LoanMonthlyInterestRepay = loanAccount.LoanInterestRatePerMonth * loanAccount.LoanPrincipalRemaining;
                            loanAccount.LoanMonthlyPrincipalRepay = loanAccount.LoanMonthlyRepay - loanAccount.LoanMonthlyInterestRepay;
                            loanAccount.LoanPrincipalRemaining = loanAccount.LoanMonthlyPrincipalRepay;
                        }

                        custActRepo.Update(loanAccount);
                        configRepo.Update(config);
                        //glRepo.Update(config.LoanInterestReceivableGl);
                        //custActRepo.Update(loanAccount.ServicingAccount);

                    }
                }
                //db.SaveChanges();
            }//end if         
        }//end method ComputeDailyLoanInterestAccrued

        public void SaveDailyIncomeAndExpenseBalance()
        {
            var allIncomes = glRepo.GetByMainCategory(MainGlCategory.Income);
            //save daily balance of all income acccounts
            foreach (var account in allIncomes)
            {
                var entry = new ExpenseIncomeEntry();
                entry.AccountName = account.AccountName;
                entry.Amount = account.AccountBalance;
                entry.Date = today;
                entry.EntryType = PandLType.Income;
                new ProfitAndLossRepository().Insert(entry);
            }

            //save daily balance off all expense accounts
            var allExpenses = glRepo.GetByMainCategory(MainGlCategory.Expenses);
            foreach (var account in allExpenses)
            {
                var entry = new ExpenseIncomeEntry();
                entry.AccountName = account.AccountName;
                entry.Amount = account.AccountBalance;
                entry.Date = today;
                entry.EntryType = PandLType.Expenses;
                new ProfitAndLossRepository().Insert(entry);
            }
        }
    }
}
