using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum TransactionType
    {
        Debit, Credit
    }

    public class Transaction
    {
        public virtual int ID { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string SubCategory { get; set; }     //eg customerAccount, CashAsset etc
        public virtual MainGlCategory MainCategory { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
