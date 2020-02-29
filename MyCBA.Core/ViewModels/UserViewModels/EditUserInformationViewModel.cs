using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.ViewModels.UserViewModels
{
    public class EditUserInformationViewModel
    {
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid Phone Number")]
        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Please enter a valid phone number"), MinLength(11), MaxLength(16)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the First Name")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "First name should  only characters and white spaces"), MaxLength(40)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the Last Name")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Last name should  only characters and white spaces"), MaxLength(40)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid Email address"), MaxLength(60)]
        public string Email { get; set; }

        public int Id { get; set; }      //user id

        [Required(ErrorMessage = "Please select a branch")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public int RoleId { get; set; }
    }
}
