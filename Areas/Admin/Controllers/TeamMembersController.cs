using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data;
using System.Data.Entity;

using SmallApp2.Data;
using SmallApp2.Models.Home;

namespace SmallApp2.Areas.Admin.Controllers
{
    public class TeamMembersController : Controller
    {
        private SmallAppEntities db = new SmallAppEntities(); //DbContext

        //
        // GET: /Admin/TeamMembers/

        public ActionResult Index()
        {
            var teamMembers = LoadTeamMembers();
            ViewBag.Teams = LoadTeamsDropDown(addAllOption: true);
            return View(teamMembers);
        }

        //
        // POST: /Admin/TeamMembers/

        [HttpPost]
        public ActionResult Index(string Teams = null, string query = null)
        {
            Guid teamId = Guid.Empty;
            if (!Guid.TryParse(Teams, out teamId)) return HttpNotFound();

            var teamMembers = LoadTeamMembers(teamId, query);

            ViewBag.Teams = LoadTeamsDropDown(teamId, true);
            return View(teamMembers);
        }

        //
        // GET: /Admin/TeamMembers/Create

        public ActionResult Create()
        {
            ViewBag.TeamId = LoadTeamsDropDown();            
            return View(new TeamMember());
        }

        //
        // POST: /Admin/TeamMembers/Create

        [HttpPost]
        public ActionResult Create(TeamMember teammember)
        {
            if (teammember.FullName == "Gerard") //TODO: Create and Edit dont share business logic validation Fix.
            {
                ViewBag.TeamId = LoadTeamsDropDown();
                ModelState.AddModelError("", "Creation was not possible. Check errors below.");
                ModelState.AddModelError("FullName", "Gerard you are not allowed.");
            }

            if (ModelState.IsValid)
            {                
                teammember.Id = Guid.NewGuid();
                db.TeamMembers.Add(teammember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teammember);
        }

        //
        // GET: /Admin/TeamMembers/Edit

        public ActionResult Edit(Guid teammemberId) //cannot be null as you can only edit a valid team member
        {
            var m = db.TeamMembers.Find(teammemberId);
            ViewBag.TeamId = LoadTeamsDropDown(m.TeamId, false); //new SelectList(db.Teams, "id", "TeamName", m.TeamId);
            return View(m);
        }

        //
        // POST: /Admin/Teammembers/Edit
        [HttpPost]
        public ActionResult Edit(TeamMember m)
        {
            ViewBag.TeamId = LoadTeamsDropDown(m.TeamId, false); //new SelectList(db.Teams, "id", "TeamName", m.TeamId);

            if (ModelState.IsValid)
            {
                var teammember = db.TeamMembers.Find(m.Id);
                if (teammember != null)
                {
                    teammember.FullName = m.FullName;
                    teammember.Age = m.Age;
                    teammember.Presentation = m.Presentation;
                    teammember.TeamId = m.TeamId;
                    teammember.Salary = m.Salary;
                    teammember.Email = m.Email;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            //TODO: show message to user to correct
            return View(m);
        }


        //
        // GET: /Admin/Teammmembers/Details

        public ActionResult Details (Guid teammemberId)
        {
            var m = (from tm in db.TeamMembers
                    where (tm.Id == teammemberId)
                    select new TeamViewModel {
                        Id = tm.Id,
                        FullName = tm.FullName,
                        Age = tm.Age,
                        Presentation = tm.Presentation,
                        TeamName = tm.Team.TeamName,
                        Salary = tm.Salary,
                        Email = tm.Email
                    }).Single<TeamViewModel>();
            return View(m);
        }

        //
        // GET: /Admin/Teammembers/Delete
        public ActionResult Delete(Guid teammemberId)
        {
            var m = (from tm in db.TeamMembers
                     where (tm.Id == teammemberId)
                     select new TeamViewModel
                     {
                         Id = tm.Id,
                         FullName = tm.FullName,
                         Age = tm.Age,
                         Presentation = tm.Presentation,
                         TeamName = tm.Team.TeamName,
                         Salary = tm.Salary,
                         Email = tm.Email
                     }).Single<TeamViewModel>();
            return View(m);
        }

        //
        // POST: /Admin/Teammembers/Delete

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid teammemberId)
        {
            var teammember = db.TeamMembers.Find(teammemberId);
            db.TeamMembers.Remove(teammember);
            db.SaveChanges();            
            return RedirectToAction("Index");
        }


        //TODO: refactor use of the team members listing on the front-end and back-end

        private SelectList LoadTeamsDropDown(Guid? selectedTeamId = null, bool addAllOption = false)
        {
            selectedTeamId = selectedTeamId ?? Guid.Empty;

            var dropdown = db.Teams.ToList();
            if (addAllOption)
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
                                  Id = m.Id,
                                  FullName = m.FullName,
                                  Age = m.Age,
                                  Presentation = m.Presentation,
                                  TeamName = m.Team.TeamName,
                                  Salary = m.Salary,
                                  Email = m.Email
                              };
            return teamMembers;
        }
    }
}
