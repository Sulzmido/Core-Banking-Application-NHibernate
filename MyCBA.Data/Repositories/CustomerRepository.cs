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
    public class CustomerRepository : BaseRepository<Customer>
    {
        public Customer GetById(string custId)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<Customer>().Where(c => c.CustId.ToLower().Equals(custId.ToLower())).FirstOrDefault();
            }
        }
    }
}
