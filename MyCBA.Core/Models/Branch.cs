using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MyCBA.Core.Models
{
    public enum BranchStatus
    {
        Closed, Open
    }
    public class Branch
    {
        public virtual int ID { get; set; }

        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string Address { get; set; }
        public virtual long SortCode { get; set; }      //autogen

        public virtual BranchStatus Status { get; set; }
    }
}
