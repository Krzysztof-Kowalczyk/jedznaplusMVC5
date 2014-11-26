using Jedznaplus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jedznaplus.Validators
{
    public class RecipeOnlyOwnerOrAdmin : AuthorizeAttribute
    {
        readonly DatabaseModel _db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                return false;
            }

            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));        
            
            var rd = httpContext.Request.RequestContext.RouteData;
            var id = Convert.ToInt32(rd.Values["id"]);
            var userName = httpContext.User.Identity.Name;

            var user = UserManager.FindByName(userName);

            if (UserManager.IsInRole(user.Id, "Admins") || UserManager.IsInRole(user.Id, "Editors")) 
            {
                return true;
            }

            var recipe = _db.Recipes.SingleOrDefault(p => p.Id == id);
         
            return recipe != null && recipe.UserName == user.UserName;
        }
    }
}