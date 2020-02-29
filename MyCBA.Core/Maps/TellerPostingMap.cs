using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class TellerPostingMap : ClassMap<TellerPosting>
    {
        public TellerPostingMap()
        {
            Id(p => p.ID);
            Map(p => p.Amount);
            Map(p => p.Date);
            Map(p => p.Narration);
            Map(p => p.PostingType);

            References(p => p.CustomerAccount).Column("CustomerAccountId").Not.LazyLoad().Not.Nullable();
            References(p => p.PostInitiator).Column("PostInitiatorId").Not.LazyLoad().Not.Nullable();

            Table("TellerPosting");
        }
    }
}
