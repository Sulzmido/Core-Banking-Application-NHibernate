using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum TellerPostingType
    {
        Deposit, Withdrawal
    }

    public class TellerPosting
    {
        public virtual int ID { get; set; }

        [Required(ErrorMessage = "You must enter an Amount")]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[0-9.]+$", ErrorMessage = "Please enter a valid amount"), Range(1, (double)Decimal.MaxValue)]
        public virtual decimal Amount { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Narration { get; set; }
        
        [DataType(DataType.Date)]
        public virtual DateTime Date { get; set; }

        [Required(ErrorMessage="Select a posting type")]
        public virtual TellerPostingType PostingType { get; set; }

        [Required(ErrorMessage = "Select an Account to make posting to")]
        [Display(Name = "Account")]
        public virtual int CustomerAccountId { get; set; }
        public virtual CustomerAccount CustomerAccount { get; set; }

        [Display(Name = "Post Initiator")]
        public virtual User PostInitiator { get; set; }
    }
}
