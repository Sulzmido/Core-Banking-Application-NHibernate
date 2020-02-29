using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public class GlPosting
    {
        public virtual int ID { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Credit Amount")]
        public virtual decimal CreditAmount { get; set; }

        [DataType(DataType.Currency)]
        [Compare("CreditAmount", ErrorMessage = "Debit Amount must be equal to Credit Amount")]
        [Display(Name = "Debit Amount")]
        public virtual decimal DebitAmount { get; set; }
        public virtual string Naration { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime Date { get; set; }


        public virtual GlAccount DrGlAccount { get; set; }

        public virtual GlAccount CrGlAccount { get; set; }

        [Display(Name = "Post Initiator")]
        public virtual User PostInitiator { get; set; }
    }
}
