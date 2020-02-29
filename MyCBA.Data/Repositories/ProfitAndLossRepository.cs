using MyCBA.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

namespace MyCBA.Data.Repositories
{
    public class ProfitAndLossRepository : BaseRepository<ExpenseIncomeEntry>
    {
        DateTime yesterday;
        public ProfitAndLossRepository()
        {
            yesterday = new ConfigurationRepository().GetFirst().FinancialDate.AddDays(-1); //since EOD there is no Expense/Income entries for today until EOD is run
        }
        public List<ExpenseIncomeEntry> GetAllExpenseIncomeEntries()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<ExpenseIncomeEntry>().ToList();
            }
        }
        public List<ExpenseIncomeEntry> GetEntries()
        {
            var result = new List<ExpenseIncomeEntry>();
            var allEntries = GetAllExpenseIncomeEntries();
            foreach (var item in allEntries)
            {
                if (item.Date.Date == yesterday.Date)
                {
                    result.Add(item);
                }
            }
            return result;
        }
        public List<ExpenseIncomeEntry> GetEntries(DateTime startDate, DateTime endDate)
        {
            var result = new List<ExpenseIncomeEntry>();
            if (startDate < endDate)
            {
                //gets all entries(with their balances) for the start and the end dates. eg: Current exp gl (bal: 3k) on Jan5, (bal 8k) on Jan 9. etc. A GL cant exist more than 2 times (for start and end dates).
                var allEntries = GetAllExpenseIncomeEntries();
                foreach (var item in allEntries)
                {
                    if (item.Date.Date == startDate || item.Date.Date == endDate)
                    {
                        result.Add(item);
                    }
                }

            }
            return result.OrderByDescending(e => e.Date).ToList();  //making entries on endDate to come before those of startDate so that the difference in Account balance between the two days could be easily calculated
        }
    }
}
