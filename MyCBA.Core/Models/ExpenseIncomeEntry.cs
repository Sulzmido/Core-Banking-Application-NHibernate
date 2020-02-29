using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Models
{
    public enum PandLType
    {
        Income, Expenses
    }
    public class ExpenseIncomeEntry
    {
        public virtual int ID { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string AccountName { get; set; }
        public virtual PandLType EntryType { get; set; }
    }
}
