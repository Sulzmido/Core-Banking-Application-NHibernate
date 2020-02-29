using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.GlPostingViewModels
{
    public class CreateGlPostViewModel
    {
        [Required(ErrorMessage = "Please enter an amount")]
        [DataType(DataType.Currency)]
        [Display(Name = "Credit Amount")]
        public decimal CreditAmount { get; set; }

        [DataType(DataType.Currency)]
        [Compare("CreditAmount", ErrorMessage = "Debit Amount must be equal to Credit Amount")]
        [Display(Name = "Debit Amount")]
        public decimal DebitAmount { get; set; }
        public string Naration { get; set; }

        [Required(ErrorMessage = "Select an account to Debit")]
        [Display(Name = "Account to Debit")]
        public int DrGlAccount_Id { get; set; }


        [Required(ErrorMessage = "Select an account to Credit")]
        [Display(Name = "Account to Credit")]
        public int CrGlAccount_Id { get; set; }
    }
}
