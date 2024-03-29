﻿using System.Globalization;
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
using PagedList;

namespace Jedznaplus.Controllers
{
    public class RecipesController : Controller
    {
        private readonly DatabaseModel _db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public RecipesController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var recipes = _db.Recipes.ToList();
            var pagedList = new PagedList<Recipe>(recipes, page, pageSize);

            return View(pagedList);
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
            int count = _db.Recipes.Count() > 3 ? 3 : _db.Recipes.Count();

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
            int count = _db.Recipes.Count() > 3 ? 3 : _db.Recipes.Count();

            var recipes = (from p in _db.Recipes
                           orderby p.AverageGrade descending
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
            ViewBag.Difficulties = ConstantStrings.Difficulties;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRecipeFirstPhaseViewModel recipe, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
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
            ViewBag.Difficulties = ConstantStrings.Difficulties;
            return Create();
        }

        [HttpGet]
        public ActionResult CreateAddIngredients(CreateRecipeFirstPhaseViewModel recipe)
        {
            var recipeToSend = new CreateRecipeSecondPhaseViewModel
            {
                Name = recipe.Name,
                ImageUrl = recipe.ImageUrl,
                PreparationTime = recipe.PreparationTime,
                Serves = recipe.Serves,
                Difficulty = recipe.Difficulty,
                Calories = recipe.Calories,
                Vegetarian = recipe.Vegetarian

            };
            return View(recipeToSend);
        }

        [HttpPost]
        [ActionName("CreateAddIngredients")]
        public ActionResult CreateAddIngredientsPost(CreateRecipeSecondPhaseViewModel recipe)
        {

            if (ModelState.IsValid)
            {
                var recipeToSave = new Recipe
                {
                    Name = recipe.Name,
                    ImageUrl = recipe.ImageUrl,
                    PreparationTime = recipe.PreparationTime,
                    Serves = recipe.Serves,
                    Difficulty = recipe.Difficulty,
                    Calories = recipe.Calories,
                    Vegetarian = recipe.Vegetarian,
                    PreparationMethod = recipe.PreparationMethod,
                    Ingredients = recipe.Ingredients,
                    UserName = User.Identity.Name,
                    CreateDate = DateTime.Now,
                    LastEditDate = DateTime.Now,
                    LastEditorName = User.Identity.Name
                };

                _db.Recipes.Add(recipeToSave);
                _db.SaveChanges();

                string changes = "Dodanie przepisu:: Id Przepisu: " + recipeToSave.Id + " | Dodany przez: " +
                                 recipeToSave.UserName + " | Czas: " + recipeToSave.CreateDate;

                Logs.SaveLog(changes);
                return RedirectToAction("UserRecipes");
            }
            ModelState.AddModelError(string.Empty,
                "Przepis musi zawierać poprawną listę składników oraz sposób przygotowania");
            return View("CreateAddIngredients", recipe);
        }


        [RecipeOnlyOwnerOrAdmin]
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

        [RecipeOnlyOwnerOrAdmin]
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

            if (dbPost.Ingredients.Count != recipe.Ingredients.Count ||
                dbPost.Ingredients.Distinct().Count() != recipe.Ingredients.Count
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


        [RecipeOnlyOwnerOrAdmin]
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
        [RecipeOnlyOwnerOrAdmin]
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
        public ActionResult UserRecipes(int page = 1, int pageSize = 10)
        {
            var userRecipes = _db.Recipes.Where(a => a.UserName == User.Identity.Name).ToList();

            var pagedList = new PagedList<Recipe>(userRecipes, page, pageSize);

            return View(pagedList);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var toView = _db.Recipes.Find(id);

            if (toView == null) return RedirectToAction("Index");

            return View(toView);
        }

        [RecipeOnlyOwnerOrAdmin]
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

        public ActionResult Search(string search, int page = 1, int pageSize = 10)
        {
            var recipes =
                _db.Recipes.Where(
                    a =>
                        a.Name.ToLower().Contains(search.ToLower()) ||
                        a.Ingredients.Any(
                            ingred => ingred.Name.ToLower().Contains(search.ToLower()))).ToList();

            var pagedList = new PagedList<Recipe>(recipes, page, pageSize);

            ViewBag.Search = search;

            return View(pagedList);
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

        public ActionResult OnlyVegetarian(int page = 1, int pageSize = 10)
        {
            var recipes = _db.Recipes.Where(p => p.Vegetarian).ToList();
            var pagedList = new PagedList<Recipe>(recipes, page, pageSize);
            return View(pagedList);
        }

        public ActionResult OnlyNonAlergic(int page = 1, int pageSize = 10)
        {
            var recipes = _db.Recipes.Where(p => p.Ingredients.All(i => i.Alergic == false)).ToList();
            var pagedList = new PagedList<Recipe>(recipes, page, pageSize);
            return View(pagedList);
        }

        [HttpPost]
        public ActionResult AdvancedSearch(ExcludeRecipe er, int page = 1, int pageSize = 10)
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
            var pagedfinalRecipes = new PagedList<Recipe>(finalRecipes, page, pageSize);
            if (er.WantedIngredients == null) return View("Search", pagedfinalRecipes);

            foreach (var rec in recipes)
            {
                hasIngred = HasWantedIngredient(rec, er);

                if (!hasIngred)
                    finalRecipes.Remove(rec);
            }

            var pagedList = new PagedList<Recipe>(finalRecipes, page, pageSize);

            ViewBag.ExcludeRecipe = er;

            return View("AdvancedSearched", pagedList);
        }

        private bool HasWantedIngredient(Recipe rec, ExcludeRecipe er)
        {
            return rec.Ingredients.Any(recIngred => er.WantedIngredients.Any
                (wIngred => recIngred.Name.IndexOf(wIngred.ToLower(), StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public string CountVotes(string votesString, string id)
        {
            var idNum = Convert.ToInt32(id);
            var recipe = _db.Recipes.Find(idNum);

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

            recipe.AverageGrade = mAverage;
            _db.SaveChanges();

            return
                "<span style=\"display: block; width: 70px; height: 13px; background: url(/Resources/Images/whitestar.gif) 0 0;\">" +
                "<span style=\"display: block; width: " + mInPercent +
                "%; height: 13px; background: url(/Resources/Images/yellowstar.gif) 0 -13px;\"></span> " +
                "</span>" +
                "<span class=\"smallText\">Ilość głosów: <span itemprop=\"ratingCount\">" + mTotalNumberOfVotes +
                "</span> | Średnia ocen : <span itemprop=\"ratingValue\">" + mAverage.ToString("##.##") +
                "</span> na 5 </span>  ";
        }


        public JsonResult CountVotesFromId(string id)
        {
            var idNum = Convert.ToInt32(id);
            var recipe = _db.Recipes.Find(idNum);
            var votesString = recipe.Votes;
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

            recipe.AverageGrade = mAverage;
            _db.SaveChanges();

            return
                Json(
                    "<span style=\"display: block; width: 70px; height: 13px; background: url(/Resources/Images/whitestar.gif) 0 0;\">" +
                    "<span style=\"display: block; width: " + mInPercent +
                    "%; height: 13px; background: url(/Resources/Images/yellowstar.gif) 0 -13px;\"></span> " +
                    "</span>" +
                    "<span class=\"smallText\">Ilość głosów: <span itemprop=\"ratingCount\">" + mTotalNumberOfVotes +
                    "</span> | Średnia ocen : <span itemprop=\"ratingValue\">" + mAverage.ToString("##.##") +
                    "</span> na 5 </span>  ");

        }

        public ActionResult IngredientEntryRow()
        {
            ViewBag.UnitNames = ConstantStrings.UnitNameList;
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
            if (quantity.Contains(','))
            {
                quantity = quantity.Replace(",", ".");
            }
            else if (quantity.Contains('/'))
            {
                string[] numbers = quantity.Split('/');
                quantity = (double.Parse(numbers[0]) / double.Parse(numbers[1])).ToString(CultureInfo.InvariantCulture);
            }
            else if (quantity.Contains('-'))
            {
                string[] numbers = quantity.Split('-');
                quantity = double.Parse(numbers[1]).ToString(CultureInfo.InvariantCulture);
            }
            double quantityNumber = double.Parse(quantity, CultureInfo.InvariantCulture);

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
                    if (Math.Abs(quantityNumber - 1) > 0.00000001 && quantityNumber > 0 && quantityNumber < 5)
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

                case "kropla":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "krople";
                    else if (quantityNumber >= 5)
                        validForm = "kropli";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "kropli";
                    break;

                case "kostka":
                    if (quantityNumber > 1 && quantityNumber < 5)
                        validForm = "kostki";
                    else if (quantityNumber >= 5)
                        validForm = "kostek";
                    else if (quantityNumber > 0 && quantityNumber < 1)
                        validForm = "kostki";
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
            var result = (from r in _db.Ingredient
                         where r.Name.ToLower().Contains(term)
                         select r.Name).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string ValidMinutesForm(int minutes)
        {

            if (minutes < 100)
            {
                if (minutes == 1)
                    return "minuta";

                if (minutes <= 4)
                    return "minuty";

                if (minutes > 4 && minutes <= 21)
                    return "minut";

                if (minutes > 21)
                {
                    if (minutes % 10 > 4 || minutes % 10 == 0 || minutes % 10 == 1)
                        return "minut";
                    else

                        return "minuty";
                }
            }
            else
            {
                if (minutes == 100 || minutes == 101)
                    return "minut";
                
                if (minutes <= 104)
                    return "minuty";

                if (minutes > 104 && minutes < 122)
                    return "minut";

                if (minutes > 121)
                {
                    if (minutes % 100 > 4 || minutes % 100 == 0 || minutes % 100 == 1)
                        return "minut";
                    else
                        return "minuty";

                }

            }
            return "minuta";
        }
    }
}
