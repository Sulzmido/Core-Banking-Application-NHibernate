using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class GlPostingMap : ClassMap<GlPosting>
    {
        public GlPostingMap()
        {
            Id(p => p.ID);
            Map(p => p.CreditAmount);
            Map(p => p.DebitAmount);
            Map(p => p.Date);
            Map(p => p.Naration);

            References(p => p.DrGlAccount).Column("DebitGlId").Not.LazyLoad().Not.Nullable();
            References(p => p.CrGlAccount).Column("CreditGlId").Not.LazyLoad().Not.Nullable();
            References(p => p.PostInitiator).Column("PostInitiatorId").Not.LazyLoad().Not.Nullable();

            Table("GlPosting");
        }
    }
}
