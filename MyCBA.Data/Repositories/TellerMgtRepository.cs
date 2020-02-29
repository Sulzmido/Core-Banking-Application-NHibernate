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
    public class TellerMgtRepository : BaseRepository<TillToUser>
    {
        public List<TillToUser> AllTellers()
        {
            var output = new List<TillToUser>();
            var tellersWithTill = GetAll();
            var tellersWithoutTill = TellersWithoutTill();

            //adding all tellers without a till account
            foreach (var teller in tellersWithoutTill)
            {
                output.Add(new TillToUser { UserId = teller.ID, TillId = 0 });
            }
            //adding all tellers with a till account
            foreach (var teller in tellersWithTill)
            {
                output.Add(new TillToUser { UserId = teller.UserId, TillId = teller.TillId });
            }

            return output;
        }

        public List<User> TellersWithoutTill()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var tellers = new UserRepository().GetAllTellers();
                //var ans = session.Query<TillToUser>().Join(tellers, t=>t.UserId, u=> u.ID, (t,u)=>new{t,u}).Where(tu => tu.u.ID != tu.t.ID);
                var output = new List<User>();
                var tillToUsers = new TellerMgtRepository().GetAll();
                foreach (var teller in tellers)
                {
                    if (!tillToUsers.Any(tu => tu.UserId == teller.ID)) //teller has no till yet
                    {
                        output.Add(teller);
                    }
                }
                return output;
            }
        }

        public List<User> TellersWithTill()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var tellers = new UserRepository().GetAllTellers();
                //var ans = session.Query<TillToUser>().Join(tellers, t=>t.UserId, u=> u.ID, (t,u)=>new{t,u}).Where(tu => tu.u.ID != tu.t.ID);
                var output = new List<User>();
                var tillToUsers = new TellerMgtRepository().GetAll();
                foreach (var teller in tellers)
                {
                    if (tillToUsers.Any(tu => tu.UserId == teller.ID)) //teller has no till yet
                    {
                        output.Add(teller);
                    }
                }
                return output;
            }
        }

        public List<GlAccount> TillsWithoutTeller()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var tills = new GlAccountRepository().GetAllTills();
                //var ans = session.Query<TillToUser>().Join(tellers, t=>t.UserId, u=> u.ID, (t,u)=>new{t,u}).Where(tu => tu.u.ID != tu.t.ID);
                var output = new List<GlAccount>();
                var tillToUsers = new TellerMgtRepository().GetAll();
                foreach (var till in tills)
                {
                    if (!tillToUsers.Any(tu => tu.TillId == till.ID)) //teller has no till yet
                    {
                        output.Add(till);
                    }
                }
                return output;
            }
        }

        public GlAccount GetUserTill(User teller)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var tellersWithTill = TellersWithTill();
                if (!tellersWithTill.Any(t => t.ID == teller.ID))
                {
                    return null;
                }
                int tillId = session.Query<TillToUser>().Where(tu => tu.UserId == teller.ID).First().TillId;
                return new GlAccountRepository().GetById(tillId);
            }
        }
    }
}
