using System.Globalization;
using Jedznaplus.Models;
using Jedznaplus.Models.ViewModels;
using Jedznaplus.Resources;
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
        private readonly DatabaseModel _db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly SelectList _unitNameList;
        private readonly SelectList _difficulties;

        public RecipesController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
            _unitNameList = new SelectList(new[] { "litr", "mililitr", "kilogram", "dekagram", "gram", "sztuka", "plaster", "opakowanie", "łyżka", "łyżeczka", "szklanka", "szczypta" });
            _difficulties = new SelectList(new[] { "Łatwy", "Średni", "Trudny", "Bardzo Trudny" });
        }

        public ActionResult Index()
        {
            var recipes = _db.Recipes.ToList();

            return View(recipes);
        }


        private void DeleteImg(string relativePath)
        {
            if (relativePath != ConstantStrings.DefaultRecipePhoto)
            {
                var path = Server.MapPath(relativePath);
                System.IO.File.Delete(path);
            }
        }

        public ActionResult NewestRecipes()
        {
            int count = _db.Recipes.Count() > 10 ? 10 : _db.Recipes.Count();

            var recipes = (from p in _db.Recipes
                           orderby p.CreateDate descending
                           select p).Take(count).ToList();

            var mv = recipes.Select(u => new NewestRecipesViewModel
            {
                Id = u.Id,
                Name = u.Name,
                ImageUrl = u.ImageUrl
            }).ToList();

            return PartialView("_NewestRecipes", mv);

        }

        public ActionResult BestRatedRecipes()
        {
            int count = _db.Recipes.Count() > 10 ? 10 : _db.Recipes.Count();

            var recipes = (from p in _db.Recipes
                           orderby p.Votes descending
                           select p).Take(count).ToList();

            var mv = recipes.Select(u => new BestRatedRecipesViewModel
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
            ViewBag.Difficulties = _difficulties;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe recipe, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                recipe.UserName = User.Identity.Name;
                recipe.CreateDate = DateTime.Now;
                recipe.LastEditDate = DateTime.Now;
                recipe.LastEditorName = User.Identity.Name;

                if (file != null && file.ContentLength > 0 && file.ContentLength < 3000000)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = Guid.NewGuid() + fileName;
                    var absolutePath = Path.Combine(Server.MapPath(ConstantStrings.RecipePhotosPath), uniqueFileName);
                    var relativePath = ConstantStrings.RecipePhotosPath + uniqueFileName;
                    file.SaveAs(absolutePath);
                    recipe.ImageUrl = relativePath;
                }
                else
                {
                    recipe.ImageUrl = ConstantStrings.DefaultRecipePhoto;
                }

                return RedirectToAction("CreateAddIngredients", recipe);
            }
            ViewBag.Difficulties = _difficulties;
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
                _db.Recipes.Add(recipe);
                _db.SaveChanges();

                string changes = "Dodanie przepisu:: Id Przepisu: " + recipe.Id + " | Dodany przez: " +
                   recipe.UserName + " | Czas: " + recipe.CreateDate;

                Logs.SaveLog(changes);
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

            var recipe = _db.Recipes.Find(id);

            var vm = new RecipeEditViewModels
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
            if (!ModelState.IsValid) return RedirectToAction("Details", new { id = recipe.Id });

            var dbPost = _db.Recipes.FirstOrDefault(p => p.Id == recipe.Id);
            if (dbPost == null)
            {
                return HttpNotFound();
            }

            dbPost.Calories = recipe.Calories;

            if (dbPost.Ingredients.Count != recipe.Ingredients.Count || dbPost.Ingredients.Distinct().Count() != recipe.Ingredients.Count
                || !(dbPost.Ingredients.All(item => recipe.Ingredients.Contains(item))))
            {
                _db.Ingredient.RemoveRange(dbPost.Ingredients);
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

            if (file != null && file.ContentLength > 0 && file.ContentLength < 3000000)
            {
                var fileName = Path.GetFileName(file.FileName);
                var uniqueFileName = Guid.NewGuid() + fileName;
                var absolutePath = Path.Combine(Server.MapPath(ConstantStrings.RecipePhotosPath), uniqueFileName);
                var relativePath = ConstantStrings.RecipePhotosPath + uniqueFileName;
                file.SaveAs(absolutePath);
                dbPost.ImageUrl = relativePath;
            }
            _db.SaveChanges();

            string changes = "Edycja przepisu:: Id Przepisu: " + recipe.Id + " | Edytowane przez: " +
                             dbPost.LastEditorName + " | Czas: " + dbPost.LastEditDate;

            Logs.SaveLog(changes);
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

            var toDelete = _db.Recipes.Find(id);

            return View(toDelete);
        }

        [HttpPost]
        [OnlyOwnerOrAdmin]
        public ActionResult Delete(int id)
        {

            var toDelete = _db.Recipes.Find(id);

            if (toDelete == null) return RedirectToAction("Index");

            var votes = _db.VoteLogs.Where(p => p.VoteForId == toDelete.Id);
            DeleteImg(toDelete.ImageUrl);
            _db.Comments.RemoveRange(toDelete.Comments);
            _db.Ingredient.RemoveRange(toDelete.Ingredients);
            _db.VoteLogs.RemoveRange(votes);
            _db.Recipes.Remove(toDelete);
            _db.SaveChanges();

            string changes = "Usunięcie przepisu:: Id Przepisu: " + toDelete.Id + " | Usunięty przez: " +
                  User.Identity.Name + " | Czas: " + DateTime.Now;

            Logs.SaveLog(changes);

            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult UserRecipes()
        {
            var userRecipes = _db.Recipes.Where(a => a.UserName == User.Identity.Name).ToList();

            return View(userRecipes);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toView = _db.Recipes.Find(id);

            if (toView == null) return RedirectToAction("Index");

            var avatar = UserManager.FindByName(toView.UserName);
            ViewBag.AvatarURL = avatar != null ? avatar.AvatarUrl : ConstantStrings.DefaultUserAvatar;

            return View(toView);
        }

        [OnlyOwnerOrAdmin]
        public ActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toDelete = _db.Recipes.Find(id);

            if (toDelete == null) return RedirectToAction("Index");

            DeleteImg(toDelete.ImageUrl);
            toDelete.ImageUrl = ConstantStrings.DefaultRecipePhoto;
            _db.SaveChanges();


            var vm = new RecipeEditViewModels
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
            var recipes = _db.Recipes.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToList();
            var allRecipes = _db.Recipes.ToList();

            foreach (var rec in from rec in allRecipes
                                from ingred in rec.Ingredients.Where
             (ingred => ingred.Name.IndexOf(search.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0).Where
             (ingred => !recipes.Contains(rec))
                                select rec)
            {
                recipes.Add(rec);
            }

            return View(recipes);
        }

        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            return View();
        }

        private bool HasExcludedIngredients(Recipe rec, ExcludeRecipe er)
        {
            return rec.Ingredients.Any(ingred => er.ExcludeIngredients.Any
                (wIngred => ingred.Name.IndexOf(wIngred.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (er.NotAlergic && ingred.Alergic)));
        }

        [HttpPost]
        public ActionResult AdvancedSearch(ExcludeRecipe er)
        {
            if (er.MaxTime == 0) er.MaxTime = 1000;

            var recipes = new List<Recipe>();
            var allRecipes = _db.Recipes.ToList();
            bool hasIngred;

            foreach (var rec in allRecipes)
            {
                hasIngred = false;
                if (rec.PreparationTime > er.MaxTime) continue;
                if (er.Vegetarians && rec.Vegetarian == false) continue;

                if (er.ExcludeIngredients != null)
                {
                    hasIngred = HasExcludedIngredients(rec, er);
                }
                else
                {
                    if (rec.Ingredients.Any(ingred => er.NotAlergic && ingred.Alergic))
                    {
                        hasIngred = true;
                    }

                }
                if (!hasIngred)
                    recipes.Add(rec);
            }

            var finalRecipes = new List<Recipe>(recipes);
            if (er.WantedIngredients == null) return View("Search", finalRecipes);

            foreach (var rec in recipes)
            {
                hasIngred = HasWantedIngredient(rec, er);

                if (!hasIngred)
                    finalRecipes.Remove(rec);
            }

            return View("Search", finalRecipes);
        }

        private bool HasWantedIngredient(Recipe rec, ExcludeRecipe er)
        {
            return rec.Ingredients.Any(recIngred => er.WantedIngredients.Any
                (wIngred => recIngred.Name.IndexOf(wIngred.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public string CountVotes(string votesString)
        {
            Single mTotalNumberOfVotes = 0;
            Single mTotalVoteCount = 0;

            // calculate total votes now
            string[] votes = votesString.Split(',');
            for (int i = 0; i < votes.Length; i++)
            {
                Single mCurrentVotesCount = int.Parse(votes[i]);
                mTotalNumberOfVotes = mTotalNumberOfVotes + mCurrentVotesCount;
                mTotalVoteCount = mTotalVoteCount + (mCurrentVotesCount * (i + 1));
            }

            float mAverage = mTotalVoteCount / mTotalNumberOfVotes;
            float mInPercent = (mAverage * 100) / 5;

            return "<span style=\"display: block; width: 70px; height: 13px; background: url(/Resources/Images/whitestar.gif) 0 0;\">" +
                  "<span style=\"display: block; width: " + mInPercent + "%; height: 13px; background: url(/Resources/Images/yellowstar.gif) 0 -13px;\"></span> " +
                  "</span>" +
                  "<span class=\"smallText\">Ilość głosów: <span itemprop=\"ratingCount\">" + mTotalNumberOfVotes + "</span> | Średnia ocen : <span itemprop=\"ratingValue\">" + mAverage.ToString("##.##") + "</span> na 5 </span>  ";

        }

        public ActionResult IngredientEntryRow()
        {
            ViewBag.UnitNames = _unitNameList;
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



        public string ValidWordForm(string unitName, string quantity)
        {
            if (quantity.Contains('.'))
            {
                quantity = quantity.Replace(".", ",");
            }
            else if (quantity.Contains('/'))
            {
                string[] numbers = quantity.Split('/');
                quantity = (double.Parse(numbers[0]) / double.Parse(numbers[1])).ToString(CultureInfo.InvariantCulture);
            }
            double quantityNumber = double.Parse(quantity);

            string validForm = unitName;
            switch (unitName)
            {
                case "litr":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "litry";
                    else if (quantityNumber >= 5)
                        validForm = "litrów";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "litra";
                    break;

                case "mililitr":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "mililitry";
                    else if (quantityNumber >= 5)
                        validForm = "mililitrów";
                    break;

                case "kilogram":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "kilogramy";
                    else if (quantityNumber >= 5)
                        validForm = "kilogramów";
                    break;

                case "dekagram":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "dekagramy";
                    else if (quantityNumber >= 5)
                        validForm = "dekagramów";
                    break;

                case "gram":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "gramy";
                    else if (quantityNumber >= 5)
                        validForm = "gramów";
                    break;

                case "sztuka":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "sztuki";
                    else if (quantityNumber >= 5)
                        validForm = "sztuk";
                    break;

                case "plaster":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "plastry";
                    else if (quantityNumber >= 5)
                        validForm = "plastrów";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "plastra";
                    break;

                case "opakowanie":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "opakowania";
                    else if (quantityNumber >= 5)
                        validForm = "opakowań";
                    break;

                case "łyżka":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "łyżki";
                    else if (quantityNumber >= 5)
                        validForm = "łyżek";
                    break;

                case "łyżeczka":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "łyżeczki";
                    else if (quantityNumber >= 5)
                        validForm = "łyżeczek";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "łyżeczeki";
                    break;

                case "szklanka":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "szklanki";
                    else if (quantityNumber >= 5)
                        validForm = "szklanek";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "szklanki";
                    break;


                case "szczypta":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "szczypty";
                    else if (quantityNumber >= 5)
                        validForm = "szczypt";
                    break;
            }
            return validForm;
        }

        public ActionResult AutoCompleteList(string term)
        {
            var result = from r in _db.Ingredient
                         where r.Name.ToLower().Contains(term)
                         select r.Name;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
