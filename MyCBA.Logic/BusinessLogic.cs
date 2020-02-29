using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class BusinessLogic
    {
        public bool IsConfigurationSet()
        {
            var config = new ConfigurationRepository().GetFirst();
            if (config.SavingsInterestExpenseGl == null || config.SavingsInterestPayableGl == null || config.CurrentInterestExpenseGl == null || config.CurrentCotIncomeGl == null || config.LoanInterestIncomeGl == null || config.LoanInterestReceivableGl == null)
            {
                // config.LoanInterestExpenseGl == null || 
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CreditGl(GlAccount account, decimal amount)
        {
            try
            {
                switch (account.GlCategory.MainCategory)
                {
                    case MainGlCategory.Asset:
                        account.AccountBalance -= amount;
                        break;
                    case MainGlCategory.Capital:
                        account.AccountBalance += amount;
                        break;
                    case MainGlCategory.Expenses:
                        account.AccountBalance -= amount;
                        break;
                    case MainGlCategory.Income:
                        account.AccountBalance += amount;
                        break;
                    case MainGlCategory.Liability:
                        account.AccountBalance += amount;
                        break;
                    default:
                        break;
                }//end switch

                //frLogic.CreateTransaction(account, amount, TransactionType.Credit);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }//end creditGl

        public bool DebitGl(GlAccount account, decimal amount)
        {
            try
            {
                switch (account.GlCategory.MainCategory)
                {
                    case MainGlCategory.Asset:
                        account.AccountBalance += amount;
                        break;
                    case MainGlCategory.Capital:
                        account.AccountBalance -= amount;
                        break;
                    case MainGlCategory.Expenses:
                        account.AccountBalance += amount;
                        break;
                    case MainGlCategory.Income:
                        account.AccountBalance -= amount;
                        break;
                    case MainGlCategory.Liability:
                        account.AccountBalance -= amount;
                        break;
                    default:
                        break;
                }//end switch
                //frLogic.CreateTransaction(account, amount, TransactionType.Debit);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }//end DebitGl

        public bool CreditCustomerAccount(CustomerAccount account, decimal amount)
        {
            try
            {
                if (account.AccountType == AccountType.Loan)
                {
                    account.AccountBalance -= amount;       //Loan accounts are assets to the bank
                }
                else
                {
                    account.AccountBalance += amount;       //Savings and current accounts are liabilities to the bank
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DebitCustomerAccount(CustomerAccount account, decimal amount)
        {
            try
            {
                if (account.AccountType == AccountType.Loan)
                {
                    account.AccountBalance += amount;
                }
                else
                {
                    account.AccountBalance -= amount;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
