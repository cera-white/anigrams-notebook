using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AnigramsNotebook.EF;

namespace AnigramsNotebook.Controllers
{
    public class ProjectsController : BaseController
    {
        // GET: Projects
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBProjects.Include(n => n.NBUnivers);
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true);
            }

            BuildIndexViewBag("Projects", projectId, showHidden);

            return View(objects.OrderByDescending(x => x.LastModifiedOn).ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBProject obj = db.NBProjects.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            BuildDetailsViewBag("Projects", id, projectId, showHidden);

            return View(obj);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            var obj = new NBProject();
            BuildUniverseDropdown(obj.NBUniverseId, showHidden);
            var currentCategory = BuildCurrentCategory("Projects");
            return View(obj);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBProjectId,NBUniverseId,Name,Description,Genre,IsActive,CreatedOn,LastModifiedOn")] NBProject obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBProjects.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBProjectId, projectId = projectId, showHidden = showHidden });
            }
            BuildUniverseDropdown(obj.NBUniverseId, showHidden);
            var currentCategory = BuildCurrentCategory("Projects");
            return View(obj);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBProject obj = db.NBProjects.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            BuildUniverseDropdown(obj.NBUniverseId, showHidden);
            var currentCategory = BuildCurrentCategory("Projects");
            return View(obj);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBProjectId,NBUniverseId,Name,Description,Genre,IsActive,CreatedOn,LastModifiedOn")] NBProject obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBProjectId, projectId = projectId, showHidden = showHidden });
            }
            BuildUniverseDropdown(obj.NBUniverseId, showHidden);
            var currentCategory = BuildCurrentCategory("Projects");
            return View(obj);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
