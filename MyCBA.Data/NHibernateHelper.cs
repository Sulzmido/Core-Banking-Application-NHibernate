using MyCBA.Core.Maps;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Data
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }
        private static void InitializeSessionFactory()
        {
             _sessionFactory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008.ConnectionString(@"Data Source=.\sqlexpress; Initial Catalog=MyCBADb; Integrated Security=true").ShowSql()).Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
            //_sessionFactory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2008.ConnectionString(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = MyCBADb; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False").ShowSql()).Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg)
            .Execute(false, true))       //(true, true) => all db data gets deleted evrytime app is run
            .BuildSessionFactory();
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
