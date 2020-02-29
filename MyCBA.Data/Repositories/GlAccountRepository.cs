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
    public class GlAccountRepository : BaseRepository<GlAccount>
    {
        public bool AnyGlIn(MainGlCategory mainCategory)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlAccount>().Any(gl => gl.GlCategory.MainCategory == mainCategory);
            }
        }
        public GlAccount GetLastGlIn(MainGlCategory mainCategory)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlAccount>().Where(g => g.GlCategory.MainCategory == mainCategory).OrderByDescending(a => a.ID).First();
            }

        }

        public GlAccount GetByName(string name)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlAccount>().Where(a => a.AccountName.ToLower().Equals(name.ToLower())).FirstOrDefault();
            }
        }

        public List<GlAccount> GetAllTills()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlAccount>().Where(a => a.AccountName.ToLower().Contains("till")).ToList();
            }
        }

        public List<GlAccount> GetByMainCategory(MainGlCategory mainCategory)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlAccount>().Where(a => a.GlCategory.MainCategory == mainCategory).ToList();
            }
        }
    }
}
