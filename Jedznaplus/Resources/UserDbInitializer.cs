using System.Data.Entity;
using Jedznaplus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Jedznaplus.Resources
{
    public class UserDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var rs = new RoleStore<IdentityRole>(context);
            var rm = new RoleManager<IdentityRole>(rs);

            rm.Create(new IdentityRole { Name = "Admins" });
            rm.Create(new IdentityRole { Name = "Users" });
            rm.Create(new IdentityRole { Name = "Editors" });

            var us = new UserStore<ApplicationUser>(context);
            var um = new UserManager<ApplicationUser>(us);

            var user = new ApplicationUser { UserName = "kalik", Email = "wargas_14@o2.pl", EmailConfirmed = true, AvatarUrl = ConstantStrings.DefaultUserAvatar };
            um.Create(user, "Admin123#");
            um.AddToRole(user.Id, "Admins");
        }
    }
}