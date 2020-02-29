using MyCBA.Core.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace MyCBA.Data.Repositories
{
    public class CustomerAccountRepository : BaseRepository<CustomerAccount>
    {
        public bool AnyAccountOfType(AccountType type)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<CustomerAccount>().Any(a => a.AccountType == type);
            }
        }
        public CustomerAccount GetByAccountNumber(long actNo)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<CustomerAccount>().Where(a => a.AccountNumber == actNo).SingleOrDefault();
            }
        }
        public List<CustomerAccount> GetByType(AccountType actType)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<CustomerAccount>().Where(a => a.AccountType == actType).ToList();
            }
        }
    }
}
