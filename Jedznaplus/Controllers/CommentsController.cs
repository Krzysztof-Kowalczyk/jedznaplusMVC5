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


                string changes = "Dodanie komentarza:: Id komentarza: " + comment.Id + "|  Id przepisu: " +
                                 comment.RecipeId + " | Dodany przez: " +
                                 comment.UserName + " | Czas: " + DateTime.Now;

                Logs.SaveLog(changes);


                if (Request.IsAjaxRequest())
                {
                    var comments = _db.Comments.Where(p => p.RecipeId == comment.RecipeId).ToList();
                    return PartialView("_CommentsList", comments);
                }
            }

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
        [CommentOnlyOwnerOrAdminOrEditors]
        public ActionResult Delete(int id)
        {
            var comment = _db.Comments.Single(p => p.Id == id);           
            try
            {

                _db.Comments.Remove(comment);
                _db.SaveChanges();

                string changes = "Usunięcie komentarza:: Id Komentarza: " + comment.Id + "|  Id Przepisu: " + comment.RecipeId + " | Usunięty przez: " +
                 User.Identity.Name + " | Czas: " + DateTime.Now;

                Logs.SaveLog(changes);

                if (Request.IsAjaxRequest())
                {
                    var comments = _db.Recipes.Single(p => p.Id == comment.RecipeId).Comments.ToList();
                    return PartialView("_CommentsList", comments);
                }


                return RedirectToAction("Details", "Recipes", new { id = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id });
            }
            catch
            {
                return RedirectToAction("Details", "Recipes", new { id = _db.Recipes.Single(p => p.Id == comment.RecipeId).Id });
            }
        }


    }
}
