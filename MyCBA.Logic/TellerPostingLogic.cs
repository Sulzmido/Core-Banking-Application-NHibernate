using MyCBA.Core.Models;
using MyCBA.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Logic
{
    public class TellerPostingLogic
    {
        BusinessLogic busLogic = new BusinessLogic();
        public string PostTeller(CustomerAccount account, GlAccount till, decimal amt, TellerPostingType pType)
        {
            string output = "";
            switch (pType)
            {
                case TellerPostingType.Deposit:
                    busLogic.CreditCustomerAccount(account, amt);
                    busLogic.DebitGl(till, amt);

                    output = "success";
                    break;
                //break;
                case TellerPostingType.Withdrawal:
                    //Transfer the money from the user's till and reflect the changes in the customer account balance
                    //check withdrawal limit

                    var config = new ConfigurationRepository().GetFirst();
                    //till = user.TillAccount;
                    if (account.AccountType == AccountType.Savings)
                    {
                        if (account.AccountBalance >= config.SavingsMinimumBalance + amt)
                        {
                            if (till.AccountBalance >= amt)
                            {
                                busLogic.CreditGl(till, amt);
                                busLogic.DebitCustomerAccount(account, amt);

                                output = "success";
                                account.SavingsWithdrawalCount++;
                            }
                            else
                            {
                                output = "Insufficient fund in the Till account";
                            }
                        }
                        else
                        {
                            output = "insufficient Balance in Customer's account, cannot withdraw!";
                        }

                    }//end if savings


                    else if (account.AccountType == AccountType.Current)
                    {
                        if (account.AccountBalance >= config.CurrentMinimumBalance + amt)
                        {
                            //REVISIT!!!
                            if (till.AccountBalance >= amt)
                            {
                                busLogic.CreditGl(till, amt);
                                busLogic.DebitCustomerAccount(account, amt);

                                output = "success";
                                //decimal x = (amt + account.CurrentLien) / 1000;
                                decimal x = (amt) / 1000;
                                decimal charge = (int)x * config.CurrentCot;
                                account.dailyInterestAccrued += charge;
                                //account.CurrentLien = (x - (int)x) * 1000;
                            }
                            else
                            {
                                output = "Insufficient fund in the Till account";
                            }
                        }
                        else
                        {
                            output = "insufficient Balance in Customer's account, cannot withdraw!";
                        }

                    }
                    else //for loan
                    {
                        output = "Please select a valid account";
                    }
                    break;
                //break;
                default:
                    break;
            }//end switch
            return output;
        }//end method PostTeller
    }
}
