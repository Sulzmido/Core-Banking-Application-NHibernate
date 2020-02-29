using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.GlViewModels
{
    public class AddGlActViewModel
    {
        [Required(ErrorMessage = "Account name is required"), MaxLength(40)]
        [Display(Name = "Account Name")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Account name should  only characters and white spaces")]
        public virtual string AccountName { get; set; }

        [Required(ErrorMessage = "Select a GL category")]
        [Display(Name = "GL category")]
        public int GlCategoryId { get; set; }

        [Required(ErrorMessage = "Select a Branch")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
    }
}
