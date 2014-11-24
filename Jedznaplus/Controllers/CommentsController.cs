using Jedznaplus.Models;
using Jedznaplus.Resources;
using Jedznaplus.Validators;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Jedznaplus.Controllers
{
    public class CommentsController : Controller
    {
        readonly DatabaseModel _db = new DatabaseModel();

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

            string changes = "Dodanie komentarza:: Id komentarza: " + comment.Id + "|  Id przepisu: " + comment.RecipeId + " | Dodany przez: " +
                             comment.UserName + " | Czas: " + DateTime.Now;

            Logs.SaveLog(changes);

            var recipeId = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            return RedirectToAction("Details", "Recipes", new { id = recipeId });
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
            var recipeId = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id;
            try
            {

                _db.Comments.Remove(comment);
                _db.SaveChanges();

                string changes = "Usunięcie komentarza:: Id Komentarza: " + comment.Id + "|  Id Przepisu: " + comment.RecipeId + " | Usunięty przez: " +
                 User.Identity.Name + " | Czas: " + DateTime.Now;

                Logs.SaveLog(changes);

                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = recipeId });
            }
        }


    }
}
