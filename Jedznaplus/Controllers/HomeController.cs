using Jedznaplus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jedznaplus.Controllers
{
    public class HomeController : Controller
    {
        DatabaseModel db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public string AvatarUrl()
        {
            return UserManager.FindByName(User.Identity.Name).AvatarUrl;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult SendRating(string r, string s, string id, string url)
        {
            int autoId = 0;
            Int16 thisVote = 0;
            Int16 sectionId = 0;
            Int16.TryParse(s, out sectionId);
            Int16.TryParse(r, out thisVote);
            int.TryParse(id, out autoId);

            if (!User.Identity.IsAuthenticated)
            {
                return Json("Nie jesteś zalogowany");
            }

            if (autoId.Equals(0))
            {
                return Json("Tej potrawy jeszcze nikt nie ocenił, bądź pierwszy");
            }

            switch (s)
            {
                case "5": // school voting
                    // check if he has already voted
                    var isIt = db.VoteLogs.Where(v => v.SectionId == sectionId &&
                        v.UserName.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) && v.VoteForId == autoId).FirstOrDefault();
                    if (isIt != null)
                    {
                        // keep the school voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(url + User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                        return Json("<br />Już oceniłeś tę potrawę !");
                    }

                    var sch = db.Recipes.Where(sc => sc.Id == autoId).FirstOrDefault();
                    if (sch != null)
                    {
                        object obj = sch.Votes;

                        string updatedVotes = string.Empty;
                        string[] votes = null;
                        if (obj != null && obj.ToString().Length > 0)
                        {
                            string currentVotes = obj.ToString(); // votes pattern will be 0,0,0,0,0
                            votes = currentVotes.Split(',');
                            // if proper vote data is there in the database
                            if (votes.Length.Equals(5))
                            {
                                // get the current number of vote count of the selected vote, always say -1 than the current vote in the array 
                                int currentNumberOfVote = int.Parse(votes[thisVote - 1]);
                                // increase 1 for this vote
                                currentNumberOfVote++;
                                // set the updated value into the selected votes
                                votes[thisVote - 1] = currentNumberOfVote.ToString();
                            }
                            else
                            {
                                votes = new string[] { "0", "0", "0", "0", "0" };
                                votes[thisVote - 1] = "1";
                            }
                        }
                        else
                        {
                            votes = new string[] { "0", "0", "0", "0", "0" };
                            votes[thisVote - 1] = "1";
                        }

                        // concatenate all arrays now
                        foreach (string ss in votes)
                        {
                            updatedVotes += ss + ",";
                        }
                        updatedVotes = updatedVotes.Substring(0, updatedVotes.Length - 1);

                        db.Entry(sch).State = EntityState.Modified;
                        sch.Votes = updatedVotes;
                        db.SaveChanges();

                        VoteLog vm = new VoteLog()
                        {
                            Active = true,
                            SectionId = Int16.Parse(s),
                            UserName = User.Identity.Name,
                            Vote = thisVote,
                            VoteForId = autoId
                        };

                        db.VoteLogs.Add(vm);

                        db.SaveChanges();

                        // keep the school voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(url + User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                    }
                    break;
                
                default:
                    break;
            }
            string starsWord = String.Empty;

            if (thisVote == 1)
                starsWord = " gwiazdkę";
            else if (thisVote == 5)
                starsWord = " gwizdek";
            else
                starsWord = " gwiazdki";

            return Json("<br />Oceniłeś potrawę na " + r + starsWord + " dziękujemy !");
        }

    }
}