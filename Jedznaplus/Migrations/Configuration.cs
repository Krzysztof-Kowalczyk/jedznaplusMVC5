namespace Jedznaplus.Migrations
{
    using Jedznaplus.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Jedznaplus.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Jedznaplus.Models.ApplicationDbContext";
        }

        protected override void Seed(Jedznaplus.Models.ApplicationDbContext context)
        {
            var rs = new RoleStore<IdentityRole>(context);
            var rm = new RoleManager<IdentityRole>(rs);

            rm.Create(new IdentityRole() { Name = "Admins" });
            rm.Create(new IdentityRole() { Name = "Users" });
            rm.Create(new IdentityRole() { Name = "Editors" });

            var us = new UserStore<ApplicationUser>(context);
            var um = new UserManager<ApplicationUser>(us);

            var user = new ApplicationUser() { UserName = "kalik", Email = "wargas_14@o2.pl", EmailConfirmed = true, AvatarUrl = "~/Images/Users/defaultavatar.png" };
            um.Create(user, "Szczur123#");
            um.AddToRole(user.Id, "Admins");
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}