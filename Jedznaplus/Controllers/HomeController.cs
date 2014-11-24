using System.Globalization;
using Jedznaplus.Models;
using Jedznaplus.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jedznaplus.Controllers
{
    public class HomeController : Controller
    {
        readonly DatabaseModel _db = new DatabaseModel();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            ApplicationDbContext = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ApplicationDbContext));
        }
        
        [OutputCache(Duration = 10)]
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration=30)]
        public ActionResult About()
        {
            ViewBag.Message = "Informacje o portalu";

            return View();
        }

        public string AvatarUrl(string userName)
        {
            var user = UserManager.FindByName(userName);
            return user != null ? user.AvatarUrl : ConstantStrings.DefaultUserAvatar;
        }
        
        [OutputCache(Duration = 30)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Dane kontaktowe";

            return View();
        }

        public JsonResult SendRating(string r, string s, string id, string url)
        {
            int autoId;
            Int16 thisVote ;
            Int16 sectionId ;
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
                    var isIt = _db.VoteLogs.FirstOrDefault(v => v.SectionId == sectionId &&
                                                               v.UserName.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase) && v.VoteForId == autoId);
                    if (isIt != null)
                    {
                        // keep the school voting flag to stop voting by this member
                        var cookie = new HttpCookie(url + User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                        return Json("<br />Już oceniłeś tę potrawę !");
                    }

                    var sch = _db.Recipes.FirstOrDefault(sc => sc.Id == autoId);
                    if (sch != null)
                    {
                        object obj = sch.Votes;

                        string updatedVotes = string.Empty;
                        string[] votes;
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
                                votes[thisVote - 1] = currentNumberOfVote.ToString(CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                votes = new[] { "0", "0", "0", "0", "0" };
                                votes[thisVote - 1] = "1";
                            }
                        }
                        else
                        {
                            votes = new[] { "0", "0", "0", "0", "0" };
                            votes[thisVote - 1] = "1";
                        }

                        // concatenate all arrays now
                        updatedVotes = votes.Aggregate(updatedVotes, (current, ss) => current + (ss + ","));
                        updatedVotes = updatedVotes.Substring(0, updatedVotes.Length - 1);

                        _db.Entry(sch).State = EntityState.Modified;
                        sch.Votes = updatedVotes;
                        _db.SaveChanges();

                        var vm = new VoteLog
                        {
                            Active = true,
                            SectionId = Int16.Parse(s),
                            UserName = User.Identity.Name,
                            Vote = thisVote,
                            VoteForId = autoId
                        };

                        _db.VoteLogs.Add(vm);

                        _db.SaveChanges();

                        // keep the school voting flag to stop voting by this member
                        var cookie = new HttpCookie(url + User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                    }
                    break;
                
                default:
                    break;
            }
            string starsWord;

            switch (thisVote)
            {
                case 1:
                    starsWord = " gwiazdkę";
                    break;
                case 5:
                    starsWord = " gwizdek";
                    break;
                default:
                    starsWord = " gwiazdki";
                    break;
            }

            return Json("<br />Oceniłeś potrawę na " + r + starsWord + " dziękujemy !");
        }

    }
}