using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace MyCBA.Data.Repositories
{
    public class BaseRepository<T> where T : class
    {
        public void Insert(T t)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction _tranaction = session.BeginTransaction())
                {
                    session.Save(t);
                    _tranaction.Commit();
                }

            }
        }
        public void InsertWithoutSaving(T t)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction _tranaction = session.BeginTransaction())
                {
                    session.Save(t);
                }
            }
        }
        public void SaveChanges()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction _tranaction = session.BeginTransaction())
                {
                    _tranaction.Commit();
                }
            }
        }
        public void Update(T t)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction _transaction = session.BeginTransaction())
                {
                    session.Update(t);
                    _transaction.Commit();
                }

            }
        }   //and save changes

        public void UpdateWithoutSaving(T t)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction _transaction = session.BeginTransaction())
                {
                    session.Update(t);
                }
            }
        }   //and save changes
        public List<T> GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<T>().ToList();
            }
        }
        public T GetFirst()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<T>().ToList().First();
            }
        }
        public T GetById(int Id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Get<T>(Id);
            }
        }
        public int GetCount()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Query<T>().Count();
            }
        }
    }
}
