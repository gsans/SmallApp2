using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmallApp2.Data;

namespace SmallApp2.Controllers
{
    public class ReferenceController : Controller
    {
        private SmallAppEntities db = new SmallAppEntities();

        //
        // GET: /Reference/

        public ActionResult Index()
        {
            var teammembers = db.TeamMembers.Include(t => t.Team);
            return View(teammembers.ToList());
        }

        //
        // GET: /Reference/Details/5

        public ActionResult Details(Guid id)
        {
            TeamMember teammember = db.TeamMembers.Find(id);
            if (teammember == null)
            {
                return HttpNotFound();
            }
            return View(teammember);
        }

        //
        // GET: /Reference/Create

        public ActionResult Create()
        {
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName");
            return View();
        }

        //
        // POST: /Reference/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamMember teammember)
        {
            if (ModelState.IsValid)
            {
                teammember.Id = Guid.NewGuid();
                db.TeamMembers.Add(teammember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", teammember.TeamId);
            return View(teammember);
        }

        //
        // GET: /Reference/Edit/5

        public ActionResult Edit(Guid id)
        {
            TeamMember teammember = db.TeamMembers.Find(id);
            if (teammember == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", teammember.TeamId);
            return View(teammember);
        }

        //
        // POST: /Reference/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamMember teammember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teammember).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", teammember.TeamId);
            return View(teammember);
        }

        //
        // GET: /Reference/Delete/5

        public ActionResult Delete(Guid id)
        {
            TeamMember teammember = db.TeamMembers.Find(id);
            if (teammember == null)
            {
                return HttpNotFound();
            }
            return View(teammember);
        }

        //
        // POST: /Reference/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TeamMember teammember = db.TeamMembers.Find(id);
            db.TeamMembers.Remove(teammember);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}