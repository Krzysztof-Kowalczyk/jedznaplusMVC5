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
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
    }
}
