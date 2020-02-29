using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.UserViewModels
{
    public class ChangeUserPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPass { get; set; }
        
        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPass { get; set; }

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPass", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPass { get; set; }
    }
}
