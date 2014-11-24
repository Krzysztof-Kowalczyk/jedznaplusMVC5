using Jedznaplus.Models;
using Jedznaplus.Validators;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Jedznaplus.Controllers
{
    public class CommentsController : Controller
    {
        readonly DatabaseModel _db = new DatabaseModel();

        // GET: Comments
  /*      public ActionResult Index(int recipeId)
        {
            var comments = _db.Comments.Where(p => p.Id == recipeId).ToList();

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
        }*/

        // POST: Comments/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreateDate = DateTime.Now;
                _db.Comments.Add(comment);
                _db.SaveChanges();
            }
            var recipeId = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            return RedirectToAction("Details", "Recipes", new {id=recipeId });
        }

        // GET: Comments/Edit/5
     /*   public ActionResult Edit(int id)
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
        }*/

        // GET: Comments/Delete/5
        [OnlyOwnerOrAdmin]
        public ActionResult Delete(int id)
        {
            var comment = _db.Comments.Single(p => p.Id == id);
            var recipeId= _db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            try
            {

                _db.Comments.Remove(comment);
                _db.SaveChanges();
                return RedirectToAction("Details", "Recipes", new { id = recipeId});
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = recipeId});
            }
        }

        // POST: Comments/Delete/5
    /*    [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var comment = _db.Comments.Single(p => p.Id == id);
            var recipeId = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            try
            {
               
                _db.Comments.Remove(comment);
                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
        }*/

    }
}
