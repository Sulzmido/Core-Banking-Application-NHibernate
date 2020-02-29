using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public class TillToUser
    {
        public virtual int ID { get; set; }

        [Required(ErrorMessage = "Select a User")]
        [Display(Name = "User")]
        public virtual int UserId { get; set; }

        [Required(ErrorMessage = "Select a Till Account")]
        [Display(Name = "Till")]
        public virtual int TillId { get; set; }
    }
}
