using MyCBA.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Data.Repositories
{
    //monitors all debit and credit transactions
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public List<Transaction> GetTrialBalanceTransactions(DateTime startDate, DateTime endDate)
        {
            var result = new List<Transaction>();
            if (startDate < endDate)
            {
                var allTransactions = GetAll();
                foreach (var item in allTransactions)
                {
                    if (item.Date.Date >= startDate && item.Date.Date <= endDate)
                    {
                        result.Add(item);
                    }
                }

            }
            return result;
        }
    }
}
