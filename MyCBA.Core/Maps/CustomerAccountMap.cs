using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class CustomerAccountMap : ClassMap<CustomerAccount>
    {
        public CustomerAccountMap()
        {
            Id(a => a.ID);
            Map(a => a.AccountNumber);
            Map(a => a.AccountName);
            Map(a => a.AccountBalance);
            Map(a => a.AccountType);
            Map(a => a.AccountStatus);
            Map(a => a.DateCreated);
            Map(a => a.DaysCount);
            Map(a => a.CurrentLien);
            Map(a => a.dailyInterestAccrued);
            Map(a => a.LoanAmount);
            Map(a => a.LoanInterestRatePerMonth);
            Map(a => a.LoanMonthlyInterestRepay);
            Map(a => a.LoanMonthlyPrincipalRepay);
            Map(a => a.LoanMonthlyRepay);
            Map(a => a.LoanPrincipalRemaining);
            Map(a => a.SavingsWithdrawalCount);
            Map(a => a.TermsOfLoan);

            References(a => a.Customer).Column("CustomerId").Not.LazyLoad().Not.Nullable();
            References(a => a.Branch).Column("BranchId").Not.LazyLoad().Not.Nullable();
            References(a => a.ServicingAccount).Column("ServicingAccountId").Not.LazyLoad().Nullable();

            Table("CustomerAccount");
        }
    }
}
