using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Id(t => t.ID);
            Map(t => t.AccountName);
            Map(t => t.Date);
            Map(t => t.Amount);
            Map(t => t.SubCategory);
            Map(t => t.MainCategory);
            Map(t => t.TransactionType);

            Table("Transactionz");
        }
    }
}
