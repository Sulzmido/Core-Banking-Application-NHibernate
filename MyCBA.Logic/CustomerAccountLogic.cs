using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class CustomerAccountLogic
    {
        CustomerAccountRepository custActRepo = new CustomerAccountRepository();
        ConfigurationRepository configRepo = new ConfigurationRepository();
        public long GenerateCustomerAccountNumber(AccountType actType, int accountHolderId)
        {
            //The account number has to relate with the customer's Id and the account type
            int startDigit = 0;
            switch (actType)
            {
                case AccountType.Savings:
                    startDigit = 1;
                    break;
                case AccountType.Current:
                    startDigit = 2;
                    break;
                case AccountType.Loan:
                    startDigit = 3;
                    break;
                default:
                    break;
            }//snd switch

            int id = accountHolderId;
            // how do i get the count of savings/current/loan accts already created for the customer ?
            long actNo = Convert.ToInt64(startDigit + "05" + id.ToString("D7"));     //05 here has no special significance.

            return actNo;
        }//end method

        public void ComputeFixedRepayment(CustomerAccount act, double nyears, double interestRate)
        {
            decimal totalAmountToRepay = 0;
            double nMonth = nyears * 12;
            double totalInterest = interestRate * nMonth * (double)act.LoanAmount;
            totalAmountToRepay = (decimal)totalInterest + (decimal)act.LoanAmount;
            act.LoanMonthlyRepay = (totalAmountToRepay / (12 * (decimal)nyears));
            act.LoanMonthlyPrincipalRepay = Convert.ToDecimal((double)act.LoanAmount / nMonth);
            act.LoanMonthlyInterestRepay = Convert.ToDecimal(totalInterest / nMonth);
            act.LoanPrincipalRemaining = (decimal)act.LoanAmount;
        }

        public void ComputeReducingRepayment(CustomerAccount act, double nyears, double interestRate)
        {
            double x = 1 - Math.Pow((1 + interestRate), -(nyears * 12));
            act.LoanMonthlyRepay = ((decimal)act.LoanAmount * (decimal)interestRate) / (decimal)x;

            act.LoanPrincipalRemaining = (decimal)act.LoanAmount;
            act.LoanMonthlyInterestRepay = (decimal)interestRate * act.LoanPrincipalRemaining;
            act.LoanMonthlyPrincipalRepay = act.LoanMonthlyRepay - act.LoanMonthlyInterestRepay;
        }

        public bool CustomerAccountHasSufficientBalance(CustomerAccount account, decimal amountToDebit)
        {
            var config = configRepo.GetFirst();
            if (account.AccountBalance >= amountToDebit + config.SavingsMinimumBalance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
