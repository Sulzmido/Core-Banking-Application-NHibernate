using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class ConfigurationMap : ClassMap<AccountConfiguration>
    {
        public ConfigurationMap()
        {
            Id(c => c.ID);
            Map(c => c.FinancialDate);
            Map(c => c.IsBusinessOpen);
            Map(c => c.SavingsCreditInterestRate);
            Map(c => c.SavingsMinimumBalance);
            Map(c => c.CurrentCreditInterestRate);
            Map(c => c.CurrentCot);
            Map(c => c.CurrentMinimumBalance);
            Map(c => c.LoanDebitInterestRate);

            References(c => c.SavingsInterestExpenseGl).Column("SavingsExpenseGlId").Not.LazyLoad().Nullable();
            References(c => c.SavingsInterestPayableGl).Column("SavingsPayableGlId").Not.LazyLoad().Nullable();
            References(c => c.CurrentInterestExpenseGl).Column("CurrentExpenseGlId").Not.LazyLoad().Nullable();
            References(c => c.CurrentCotIncomeGl).Column("CurrentCotIncomeGlId").Not.LazyLoad().Nullable();
            References(c => c.LoanInterestExpenseGl).Column("LoanExpenseGlId").Not.LazyLoad().Nullable();
            References(c => c.LoanInterestReceivableGl).Column("LoanReceivableGlId").Not.LazyLoad().Nullable();
            References(c => c.LoanInterestIncomeGl).Column("LoanIncomeGlId").Not.LazyLoad().Nullable();

            Table("AccountConfiguration");
        }
    }
}
