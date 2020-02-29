using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum MainGlCategory
    {
        Asset, Capital, Expenses, Income, Liability
    }
    public class GlCategory
    {
        public virtual int ID { get; set; }

        [Required(ErrorMessage = ("Category name is required")), MaxLength(40)]
        [Display(Name = "Name")]
        public virtual string Name { get; set; }

        [Required(ErrorMessage = ("Please eneter a description")), MaxLength(150)]
        [Display(Name = "Description")]
        public virtual string Description { get; set; }

        [Display(Name = "Main GL Category")]
        [Required(ErrorMessage = "You have to Select the main GL Category")]
        public virtual MainGlCategory MainCategory { get; set; }
    }
}
