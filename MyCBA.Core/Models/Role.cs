using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MyCBA.Core.Models
{
    public class Role
    {
        public virtual int ID { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Role name should only contain characters"), MaxLength(40)]
        public virtual string Name { get; set; }    //unique
    }
}
