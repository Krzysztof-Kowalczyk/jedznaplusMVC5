using Jedznaplus.Models;
using Jedznaplus.Validators;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Jedznaplus.Controllers
{
    public class CommentsController : Controller
    {
        DatabaseModel db = new DatabaseModel();

        // GET: Comments
        public ActionResult Index(int RecipeId)
        {
            var comments = db.Comments.Where(p => p.Id == RecipeId).ToList();

            return View("CommentIndex", comments);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreateDate = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            var recipeId = db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            return RedirectToAction("Details", "Recipes", new {id=recipeId });
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Comments/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Comments/Delete/5
        [OnlyOwnerOrAdmin]
        public ActionResult Delete(int id)
        {
            var comment = db.Comments.Single(p => p.Id == id);
            var recipeId= db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            try
            {

                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Recipes", new { id = recipeId});
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = recipeId});
            }
        }

        // POST: Comments/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var comment = db.Comments.Single(p => p.Id == id);
            var recipeId = db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            try
            {
               
                db.Comments.Remove(comment);
                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
        }

    }
}
