using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.CustomerAccountViewModels
{
    public class EditCustomerAccountViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Account Name")]
        [Display(Name = "Account Name")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Account name should  only characters and white spaces"), MaxLength(40)]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Select a Branch")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
    }//end editCustomer
}
