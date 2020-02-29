using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class GlAccountMap : ClassMap<GlAccount>
    {
        public GlAccountMap()
        {
            Id(g => g.ID);
            Map(g => g.AccountName);
            Map(g => g.CodeNumber);
            Map(g => g.AccountBalance);
            References(g => g.GlCategory).Column("GlCategoryId").Not.LazyLoad().Not.Nullable();
            References(g => g.Branch).Column("BranchId").Not.LazyLoad().Not.Nullable();

            Table("GlAccount");
        }
    }
}
