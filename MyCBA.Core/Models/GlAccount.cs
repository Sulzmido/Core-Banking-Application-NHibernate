using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public class GlAccount
    {
        public virtual int ID { get; set; }

        [Required(ErrorMessage = "Account name is required"), MaxLength(40)]
        [Display(Name = "Account Name")]
        public virtual string AccountName { get; set; }

        [Display(Name = "Code")]
        public virtual long CodeNumber { get; set; }     //auto-generate this, startwith(1 for Assets, 2 for Liablities, 3 for Capital, 4 for Income and 5 for Expenses)! Are these categories??

        public virtual decimal AccountBalance { get; set; } //this may be Cr or Dr balance depending on the GL main category

        [Display(Name = "Category")]
        //public int GlCategoryId { get; set; }
        public virtual GlCategory GlCategory { get; set; }

        [Required(ErrorMessage = "Please select a branch")]
        [Display(Name = "Branch")]
        //public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
