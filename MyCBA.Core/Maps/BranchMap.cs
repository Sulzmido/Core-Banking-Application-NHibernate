using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyCBA.Core.Models;

namespace MyCBA.Core.Maps
{
    class BranchMap : ClassMap<Branch>
    {
        public BranchMap()
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Address);
            Map(x => x.SortCode);
            Map(x => x.Status);

            Table("Branch");
        }
    }
}
