using System.Web.Mvc;
using System.Web.Routing;

namespace Jedznaplus
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //**********************Ignore Routes**********************
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //**********************RecipesController Routes**********************
            routes.MapRoute(
                "Przepisy",
                "Przepisy",
                new { controller = "Recipes", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/Wegetarianskie",
                "Przepisy/Wegetarianskie",
                new { controller = "Recipes", action = "OnlyVegetarian", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/DlaAlergikow",
                "Przepisy/DlaAlergikow",
                new { controller = "Recipes", action = "OnlyNonAlergic", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/TwojePrzepisy",
                "Przepisy/TwojePrzepisy",
                new { controller = "Recipes", action = "UserRecipes", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/WyszukiwanieZaawansowane",
                "Przepisy/WyszukiwanieZaawansowane",
                new { controller = "Recipes", action = "AdvancedSearch", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/NowyPrzepis",
                "Przepisy/NowyPrzepis",
                new { controller = "Recipes", action = "Create", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/Szczegoly",
                "Przepisy/Szczegoly/{id}",
                new { controller = "Recipes", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Przepisy/Edytuj",
                "Przepisy/Edytuj/{id}",
                new { controller = "Recipes", action = "Edit", id = UrlParameter.Optional }
            );

            //**********************HomeController Routes**********************
            routes.MapRoute(
                "O_stronie",
                "O_stronie",
                new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Kontakt",
                "Kontakt",
                new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
            );

            //**********************AccountController Routes**********************
            routes.MapRoute(
                "Konto/Logowanie",
                "Konto/Logowanie",
                new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Konto/Rejestracja",
                "Konto/Rejestracja",
                new { controller = "Account", action = "Register", id = UrlParameter.Optional }
            );


            //**********************ManageController Routes**********************
            routes.MapRoute(
                "Zarzadzanie",
                "Zarzadzanie",
                new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Zarzadzanie/ZmienAvatar",
                "Zarzadzanie/ZmienAvatar",
                new { controller = "Manage", action = "ChangeAvatar", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Zarzadzanie/ZmienHaslo",
                "Zarzadzanie/ZmienHaslo",
                new { controller = "Manage", action = "ChangePassword", id = UrlParameter.Optional }
            );

            //**********************Default Route**********************
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
