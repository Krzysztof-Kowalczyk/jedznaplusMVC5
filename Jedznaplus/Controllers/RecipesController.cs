using Jedznaplus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jedznaplus.Validators;

namespace Jedznaplus.Controllers
{
    public class RecipesController : Controller
    {

        private DatabaseModel db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private SelectList UnitNameList;
        private SelectList Difficulties;

        public RecipesController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            UnitNameList = new SelectList(new[] { "litr", "mililitr", "kilogram", "dekagram", "gram", "sztuka", "plaster", "opakowanie", "łyżka", "łyżeczka", "szklanka", "szczypta" });
            Difficulties = new SelectList(new[] { "Łatwy", "Średni", "Trudny", "Bardzo Trudny" });
        }

        public ActionResult Index()
        {
            var recipes = db.Recipes.ToList();

            return View(recipes);
        }


        public void deleteImg(string relativePath)
        {
            if (relativePath != "~/Images/noPhoto.png")
            {
                var path = Server.MapPath(relativePath);
                System.IO.File.Delete(path);
            }
        }

        public ActionResult NewestRecipes()
        {
            int count = db.Recipes.Count() > 10 ? 10 : db.Recipes.Count();

            var recipes = (from p in db.Recipes
                           orderby p.CreateDate descending
                           select p).Take(count).ToList();

            var mv = recipes.Select(u => new NewestRecipesViewModel()
            {
                Id = u.Id,
                Name = u.Name,
                ImageUrl = u.ImageUrl
            }).ToList();

            return PartialView("_NewestRecipes", mv);

        }

        public ActionResult BestRatedRecipes()
        {
            int count = db.Recipes.Count() > 10 ? 10 : db.Recipes.Count();

            var recipes = (from p in db.Recipes
                           orderby p.Votes descending
                           select p).Take(count).ToList();

            var mv = recipes.Select(u => new BestRatedRecipesViewModel()
            {
                Id = u.Id,
                Name = u.Name,
                ImageUrl = u.ImageUrl
            }).ToList();

            return PartialView("_BestRatedRecipes", mv);

        }


        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Difficulties = Difficulties;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Recipe recipe, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                recipe.UserName = User.Identity.Name;
                recipe.CreateDate = DateTime.Now;
                recipe.LastEditDate = DateTime.Now;
                recipe.LastEditorName = User.Identity.Name;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = Guid.NewGuid() + fileName;
                    var absolutePath = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                    var relativePath = "~/Images/" + uniqueFileName;
                    file.SaveAs(absolutePath);
                    recipe.ImageUrl = relativePath;
                }
                else
                {
                    recipe.ImageUrl = "~/Images/noPhoto.png";
                }

                return RedirectToAction("CreateAddIngredients", recipe);
            }
            ViewBag.Difficulties = Difficulties;
            return Create();
        }

        [HttpGet]
        public ActionResult CreateAddIngredients(Recipe recipe)
        {
            return View(recipe);
        }

        [HttpPost]
        [ActionName("CreateAddIngredients")]
        public ActionResult CreateAddIngredientsPost(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Przepis musi zawierać poprawną listę składników oraz sposób przygotowania");
            return View("CreateAddIngredients", recipe);
        }



        [OnlyOwnerOrAdmin]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var recipe = db.Recipes.Find(id);

            var vm = new RecipeEditViewModels()
            {
                Id = recipe.Id,
                Calories = recipe.Calories,
                Difficulty = recipe.Difficulty,
                ImageUrl = recipe.ImageUrl,
                Ingredients = recipe.Ingredients,
                Name = recipe.Name,
                PreparationMethod = recipe.PreparationMethod,
                PreparationTime = recipe.PreparationTime,
                Serves = recipe.Serves,
                Vegetarian = recipe.Vegetarian,
            };

            return View(vm);
        }

        [OnlyOwnerOrAdmin]
        [HttpPost]
        public ActionResult Edit(RecipeEditViewModels recipe, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var dbPost = db.Recipes.FirstOrDefault(p => p.Id == recipe.Id);
                if (dbPost == null)
                {
                    return HttpNotFound();
                }

                dbPost.Calories = recipe.Calories;

                if (dbPost.Ingredients.Count != recipe.Ingredients.Count || dbPost.Ingredients.Distinct().Count() != recipe.Ingredients.Count
                    || !(dbPost.Ingredients.All(item => recipe.Ingredients.Contains(item))))
                {
                    db.Ingredient.RemoveRange(dbPost.Ingredients);
                    dbPost.Ingredients = new List<Ingredient>(recipe.Ingredients);
                }

                dbPost.Name = recipe.Name;
                dbPost.PreparationMethod = recipe.PreparationMethod;
                dbPost.PreparationTime = recipe.PreparationTime;
                dbPost.Serves = recipe.Serves;
                dbPost.Difficulty = recipe.Difficulty;
                dbPost.Vegetarian = recipe.Vegetarian;
                dbPost.LastEditDate = DateTime.Now;
                dbPost.LastEditorName = User.Identity.Name;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = Guid.NewGuid() + fileName;
                    var absolutePath = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                    var relativePath = "~/Images/" + uniqueFileName;
                    file.SaveAs(absolutePath);
                    dbPost.ImageUrl = relativePath;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = recipe.Id });
        }


        [OnlyOwnerOrAdmin]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toDelete = db.Recipes.Find(id);

            return View(toDelete);
        }

        [HttpPost]
        [OnlyOwnerOrAdmin]
        public ActionResult Delete(int id)
        {

            var toDelete = db.Recipes.Find(id);

            if (toDelete != null)
            {
                var votes = db.VoteLogs.Where(p => p.VoteForId == toDelete.Id);
                deleteImg(toDelete.ImageUrl);
                db.Comments.RemoveRange(toDelete.Comments);
                db.Ingredient.RemoveRange(toDelete.Ingredients);
                db.VoteLogs.RemoveRange(votes);
                db.Recipes.Remove(toDelete);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult UserRecipes()
        {
            var userRecipes = db.Recipes.Where(a => a.UserName == User.Identity.Name).ToList();

            return View(userRecipes);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toView = db.Recipes.Find(id);

            if (toView != null)
            {
                var avatar = UserManager.FindByName(toView.UserName);
                ViewBag.AvatarURL = avatar != null ? avatar.AvatarUrl : "~/Images/Users/defaultavatar.png";

                return View(toView);
            }

            return RedirectToAction("Index");
        }

        [OnlyOwnerOrAdmin]
        public ActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toDelete = db.Recipes.Find(id);

            if (toDelete != null)
            {
                deleteImg(toDelete.ImageUrl);
                toDelete.ImageUrl = "~/Images/noPhoto.png";
                db.SaveChanges();
            }

            var vm = new RecipeEditViewModels()
            {
                Id = toDelete.Id,
                Calories = toDelete.Calories,
                Difficulty = toDelete.Difficulty,
                ImageUrl = toDelete.ImageUrl,
                Ingredients = toDelete.Ingredients,
                Name = toDelete.Name,
                PreparationMethod = toDelete.PreparationMethod,
                PreparationTime = toDelete.PreparationTime,
                Serves = toDelete.Serves,
                Vegetarian = toDelete.Vegetarian,
            };

            return RedirectToAction("Edit", vm);

        }

        public ActionResult Search(string search)
        {
            var recipes = db.Recipes.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToList();
            var allRecipes = db.Recipes.ToList();

            foreach (var rec in allRecipes)
            {
                foreach (var ingred in rec.Ingredients)
                {
                    if (ingred.Name.IndexOf(search.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (!recipes.Contains(rec))
                            recipes.Add(rec);
                    }
                }
            }

            return View(recipes);
        }

        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            return View();
        }

        public bool hasExcludedIngredients(Recipe rec, ExcludeRecipe er)
        {
            foreach (var ingred in rec.Ingredients)
            {
                foreach (var wIngred in er.ExcludeIngredients)
                {
                    if (ingred.Name.IndexOf(wIngred.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0 || (er.NotAlergic == true && ingred.Alergic == true))
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        [HttpPost]
        public ActionResult AdvancedSearch(ExcludeRecipe er)
        {
            if (er.MaxTime == 0) er.MaxTime = 1000;

            var recipes = new List<Recipe>();
            var allRecipes = db.Recipes.ToList();
            bool hasIngred;

            foreach (var rec in allRecipes)
            {
                hasIngred = false;
                if (rec.PreparationTime > er.MaxTime) continue;
                if (er.Vegetarians == true && rec.Vegetarian == false) continue;

                if (er.ExcludeIngredients != null)
                {
                    hasIngred = hasExcludedIngredients(rec, er);
                }
                else
                {
                    foreach (var ingred in rec.Ingredients)
                    {
                        if (er.NotAlergic == true && ingred.Alergic == true)
                        {
                            hasIngred = true;
                            break;
                        }

                    }

                }
                if (!hasIngred)
                    recipes.Add(rec);
            }

            var finalRecipes = new List<Recipe>(recipes);
            if (er.WantedIngredients != null)
            {
                foreach (var rec in recipes)
                {
                    hasIngred = false;
                    hasIngred = hasWantedIngredient(rec, er);

                    if (!hasIngred)
                        finalRecipes.Remove(rec);
                }
            }

            return View("Search", finalRecipes);
        }

        public bool hasWantedIngredient(Recipe rec, ExcludeRecipe er)
        {
            foreach (var recIngred in rec.Ingredients)
            {
                foreach (var wIngred in er.WantedIngredients)
                {
                    if (recIngred.Name.IndexOf(wIngred.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        public string CountVotes(string votesString)
        {
            Single m_Average = 0;

            Single m_totalNumberOfVotes = 0;
            Single m_totalVoteCount = 0;
            Single m_currentVotesCount = 0;
            Single m_inPercent = 0;

            // calculate total votes now
            string[] votes = votesString.Split(',');
            for (int i = 0; i < votes.Length; i++)
            {
                m_currentVotesCount = int.Parse(votes[i]);
                m_totalNumberOfVotes = m_totalNumberOfVotes + m_currentVotesCount;
                m_totalVoteCount = m_totalVoteCount + (m_currentVotesCount * (i + 1));
            }

            m_Average = m_totalVoteCount / m_totalNumberOfVotes;
            m_inPercent = (m_Average * 100) / 5;

            return "<span style=\"display: block; width: 70px; height: 13px; background: url(/Images/whitestar.gif) 0 0;\">" +
                  "<span style=\"display: block; width: " + m_inPercent + "%; height: 13px; background: url(/Images/yellowstar.gif) 0 -13px;\"></span> " +
                  "</span>" +
                  "<span class=\"smallText\">Ilość głosów: <span itemprop=\"ratingCount\">" + m_totalNumberOfVotes + "</span> | Średnia ocen : <span itemprop=\"ratingValue\">" + m_Average.ToString("##.##") + "</span> na 5 </span>  ";

        }

        public ActionResult IngredientEntryRow()
        {
            ViewBag.UnitNames = UnitNameList;
            return PartialView("_IngredientEditor");
        }

        public ActionResult ExcludeIngredientEntryRow()
        {
            return PartialView("_ExcludeIngredientEditor");
        }

        public ActionResult WantedIngredientEntryRow()
        {
            return PartialView("_WantedIngredientEditor");
        }



        public string validWordForm(string unitName, string quantity)
        {
            if (quantity.Contains('.'))
            {
                quantity = quantity.Replace(".", ",");
            }
            else if (quantity.Contains('/'))
            {
                string[] numbers = quantity.Split('/');
                quantity = (double.Parse(numbers[0]) / double.Parse(numbers[1])).ToString();
            }
            double Quantity = double.Parse(quantity);

            string validForm = unitName;
            switch (unitName)
            {
                case "litr":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "litry";
                    else if (Quantity >= 5)
                        validForm = "litrów";
                    else if (Quantity > 0 && Quantity < 1)
                        validForm = "litra";
                    break;

                case "mililitr":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "mililitry";
                    else if (Quantity >= 5)
                        validForm = "mililitrów";
                    break;

                case "kilogram":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "kilogramy";
                    else if (Quantity >= 5)
                        validForm = "kilogramów";
                    break;

                case "dekagram":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "dekagramy";
                    else if (Quantity >= 5)
                        validForm = "dekagramów";
                    break;

                case "gram":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "gramy";
                    else if (Quantity >= 5)
                        validForm = "gramów";
                    break;

                case "sztuka":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "sztuki";
                    else if (Quantity >= 5)
                        validForm = "sztuk";
                    break;

                case "plaster":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "plastry";
                    else if (Quantity >= 5)
                        validForm = "plastrów";
                    else if (Quantity > 0 && Quantity < 1)
                        validForm = "plastra";
                    break;

                case "opakowanie":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "opakowania";
                    else if (Quantity >= 5)
                        validForm = "opakowań";
                    break;

                case "łyżka":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "łyżki";
                    else if (Quantity >= 5)
                        validForm = "łyżek";
                    break;

                case "łyżeczka":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "łyżeczki";
                    else if (Quantity >= 5)
                        validForm = "łyżeczek";
                    else if (Quantity > 0 && Quantity < 1)
                        validForm = "łyżeczeki";
                    break;

                case "szklanka":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "szklanki";
                    else if (Quantity >= 5)
                        validForm = "szklanek";
                    else if (Quantity > 0 && Quantity < 1)
                        validForm = "szklanki";
                    break;


                case "szczypta":
                    if (Quantity > 1 && Quantity < 5)
                        validForm = "szczypty";
                    else if (Quantity >= 5)
                        validForm = "szczypt";
                    break;
            }
            return validForm;
        }

    }
}
