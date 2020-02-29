using MyCBA.Core.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCBA.Core.Maps
{
    class GlCategoryMap : ClassMap<GlCategory>
    {
        public GlCategoryMap()
        {
            Id(c => c.ID);
            Map(c => c.Name);
            Map(c => c.Description);
            Map(c => c.MainCategory);

            Table("GlCategory");
        }
    }
}
