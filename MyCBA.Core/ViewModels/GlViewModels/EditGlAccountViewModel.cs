using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.GlViewModels
{
    public class EditGlAccountViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Account Name is required!")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Account name should  only characters and white spaces"), MaxLength(40)]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Select a branch")]
        public int BranchId { get; set; }
    }
}
