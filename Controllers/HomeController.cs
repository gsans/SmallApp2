using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.Data;
using System.Data.Entity;
using SmallApp2.Models.Home;

using SmallApp2.Data;

namespace SmallApp2.Controllers
{
    public class HomeController : Controller
    {
        private SmallAppEntities db = new SmallAppEntities(); //DbContext

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Team()
        {
            var teamMembers = LoadTeamMembers();
            ViewBag.Teams = LoadTeamsDropDown();  
            return View(teamMembers);
        }

        [HttpPost]
        public ActionResult Team(string Teams = null, string query = null)
        {
            Guid teamId= Guid.Empty;
            if (!Guid.TryParse(Teams, out teamId)) return HttpNotFound();

            var teamMembers = LoadTeamMembers(teamId, query);

            ViewBag.Teams = LoadTeamsDropDown(teamId);
            return View(teamMembers);
        }

        private SelectList LoadTeamsDropDown(Guid? selectedTeamId = null)
        {
            selectedTeamId = selectedTeamId ?? Guid.Empty;

            var dropdown = db.Teams.ToList();
            dropdown.Add(new Team { Id = Guid.Empty, TeamName = "All" });
            dropdown = dropdown.OrderBy(x => x.TeamName).ToList();
            return new SelectList(dropdown, "Id", "TeamName", selectedTeamId);           //default value now is TeamId 
        }

        private IQueryable LoadTeamMembers(Guid? selectedTeamId = null, string query = null)
        {
            selectedTeamId = selectedTeamId ?? Guid.Empty;
            var teamMembers = from m in db.TeamMembers.Include(t => t.Team).Where(t => (t.TeamId == selectedTeamId) || (selectedTeamId == Guid.Empty))
                              where (m.FullName.IndexOf(query) >= 0 || string.IsNullOrEmpty(query.Trim()))
                              select new TeamViewModel
                              {
                                  FullName = m.FullName,
                                  Age = m.Age,
                                  Presentation = m.Presentation,
                                  TeamName = m.Team.TeamName
                              };
            return teamMembers;
        }
    }
}
