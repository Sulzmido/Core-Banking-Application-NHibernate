using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum AccountType
    {
        Savings, Current, Loan
    }
    public enum AccountStatus
    {
        Active, Closed
    }
    public enum TermsOfLoan
    {
        Fixed, Reducing
    }
    public class CustomerAccount
    {
        public virtual int ID { get; set; }
        public virtual long AccountNumber { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Account name should  only characters and white spaces"), MaxLength(40)]
        [Display(Name = "Account Name")]
        public virtual string AccountName { get; set; }

        [Required(ErrorMessage = "Please select a branch")]
        [Display(Name = "Branch")]
        public virtual int BranchId { get; set; }
        public virtual Branch Branch { get; set; }


        [Display(Name = "Account Balance")]
        [DataType(DataType.Currency)]
        public virtual decimal AccountBalance { get; set; }

        [Display(Name = "Date Created")]
        public virtual DateTime DateCreated { get; set; }

        public virtual int DaysCount { get; set; }      //counts the number of days (at EOD) from account creation, (esp for loan accounts)

        public virtual decimal dailyInterestAccrued { get; set; }

        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 an 100")]
        public virtual decimal LoanInterestRatePerMonth { get; set; }

        [Display(Name = "Account Type")]
        [Required(ErrorMessage = "You must select an account type")]
        public virtual AccountType AccountType { get; set; }

        public virtual AccountStatus AccountStatus { get; set; }
        public virtual int SavingsWithdrawalCount { get; set; }
        public virtual decimal CurrentLien { get; set; }       //holding amount

        [Display(Name = "Customer Id")]
        [Required(ErrorMessage = "Select a customer"), MinLength(8), MaxLength(8)]
        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Please enter a valid Customer Id")]
        public virtual string CustId { get; set; }
        public virtual Customer Customer { get; set; }



        //for a loan account
        [DataType(DataType.Currency)]
        public virtual decimal LoanAmount { get; set; }
        public virtual decimal LoanMonthlyRepay { get; set; }
        public virtual decimal LoanMonthlyInterestRepay { get; set; }
        public virtual decimal LoanMonthlyPrincipalRepay { get; set; }
        public virtual decimal LoanPrincipalRemaining { get; set; }
        public virtual TermsOfLoan? TermsOfLoan { get; set; }

        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Please enter a valid Account Number")]
        [Display(Name = "Linked Account Number")]
        public virtual int ServicingAccountId { get; set; }
        public virtual CustomerAccount ServicingAccount { get; set; }
    }
}
