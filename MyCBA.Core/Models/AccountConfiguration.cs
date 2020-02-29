using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public class AccountConfiguration
    {
        public virtual int ID { get; set; }
        public virtual bool IsBusinessOpen { get; set; }

        public virtual DateTime FinancialDate { get; set; }

        [Display(Name = "Savings Credit Interest Rate")]
        [Range(0, 100)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format for interest rate")]
        public virtual double SavingsCreditInterestRate { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format for minimum balance")]
        public virtual decimal SavingsMinimumBalance { get; set; }


        public virtual GlAccount SavingsInterestExpenseGl { get; set; }


        public virtual GlAccount SavingsInterestPayableGl { get; set; }     //Liability


        //current account side
        [Display(Name = "Current Credit Interest Rate")]
        [Range(0, 100)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format")]
        public virtual double CurrentCreditInterestRate { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format")]
        public virtual decimal CurrentMinimumBalance { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "COT")]
        [Range(0.00, 1000.00)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format")]
        public virtual decimal CurrentCot { get; set; }        //Commission On Turnover


        public virtual GlAccount CurrentInterestExpenseGl { get; set; }


        public virtual GlAccount CurrentCotIncomeGl { get; set; }


        //Loan account side
        [Display(Name = "Loan Debit Interest Rate")]
        [Range(0, 100)]
        [RegularExpression(@"^[.0-9]+$", ErrorMessage = "Invalid format")]
        public virtual double LoanDebitInterestRate { get; set; }


        public virtual GlAccount LoanInterestIncomeGl { get; set; }

        public virtual GlAccount LoanInterestExpenseGl { get; set; }        //Expense: from where the loan is disbursed

        public virtual GlAccount LoanInterestReceivableGl { get; set; }     //Asset
    }
}
