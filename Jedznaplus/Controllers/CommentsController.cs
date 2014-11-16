using Jedznaplus.Models;
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
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Recipes", db.Recipes.Single(p => p.Id == comment.RecipeId));
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Comments/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
