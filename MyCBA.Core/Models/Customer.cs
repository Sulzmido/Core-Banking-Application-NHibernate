using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum CustomerStatus { Active, Inactive }
    public class Customer
    {
        public virtual int ID { get; set; }

        [Display(Name = "Customer ID")]
        public virtual string CustId { get; set; }      //an 8-digit customer ID

        [Required(ErrorMessage = "You must enter a Full Name")]
        [RegularExpression(@"^[ a-zA-Z]+$", ErrorMessage = "Full name should only characters and white spaces"), MaxLength(40)]
        [Display(Name = "Full Name")]
        public virtual string FullName { get; set; }

        [DataType(DataType.Text), MaxLength(100)]
        [Required]
        public virtual string Address { get; set; }


        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address"), MaxLength(100)]
        public virtual string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Please enter a valid Phone Number")]
        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Please enter a valid phone number"), MinLength(11), MaxLength(16)]
        [Display(Name = "Phone number")]
        public virtual string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a gender")]
        public virtual string Gender { get; set; }

        public virtual CustomerStatus Status { get; set; }
    }
}
