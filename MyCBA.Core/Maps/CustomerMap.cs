using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyCBA.Core.Models;

namespace MyCBA.Core.Maps
{
    class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(c => c.ID);
            Map(c => c.CustId);
            Map(c => c.FullName);
            Map(c => c.PhoneNumber);
            Map(c => c.Email);
            Map(c => c.Gender);
            Map(c => c.Address);
            Map(c => c.Status);

            Table("Customer");
        }
    }
}
