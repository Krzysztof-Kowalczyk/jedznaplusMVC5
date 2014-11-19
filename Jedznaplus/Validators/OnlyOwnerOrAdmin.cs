using Jedznaplus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jedznaplus.Validators
{
    public class OnlyOwnerOrAdmin : AuthorizeAttribute
    {
        DatabaseModel db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                return false;
            }

            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));        
            
            var rd = httpContext.Request.RequestContext.RouteData;
            var id = Convert.ToInt32(rd.Values["id"]);
            var userName = httpContext.User.Identity.Name;

            ApplicationUser user = UserManager.FindByName(userName);

            if(UserManager.IsInRole(user.Id,"Admins")) 
            {
                return true;
            }

            Recipe recipe = db.Recipes.SingleOrDefault(p => p.Id == id);

            return recipe.UserName == user.UserName;
        }
    }
}