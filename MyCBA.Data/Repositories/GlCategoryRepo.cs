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
    public class GlCategoryRepo : BaseRepository<GlCategory>
    {
        public GlCategory GetByName(string name)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<GlCategory>().Where(c => c.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
            }
        }
    }
}
