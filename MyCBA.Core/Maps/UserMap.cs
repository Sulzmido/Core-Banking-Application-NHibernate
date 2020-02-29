using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyCBA.Core.Models;

namespace MyCBA.Core.Maps
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(u => u.ID);
            Map(u => u.Username);
            Map(u => u.PasswordHash);
            Map(u => u.FirstName);
            Map(u => u.LastName);
            Map(u => u.Email);
            Map(u => u.PhoneNumber);
            Map(u => u.EmailConfirmed);
            Map(u => u.VerificationCode);
            Map(u => u.TokenExpiryDate);

            References(u => u.Branch).Column("BranchId").Not.LazyLoad().Not.Nullable();
            References(u => u.Role).Column("RoleId").Not.LazyLoad().Not.Nullable();

            Table("Users");
        }
    }
}
