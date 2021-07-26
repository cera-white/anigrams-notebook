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
    public class ScenesController : BaseController
    {
        // GET: Scenes
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBScenes.Include(n => n.NBProject);
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true);
            }

            if (projectId != null && projectId > 0)
            {
                objects = objects.Where(x => x.NBProjectId == projectId);
            }

            BuildIndexViewBag("Scenes", projectId, showHidden);

            return View(objects.OrderByDescending(x => x.LastModifiedOn).ToList());
        }

        // GET: Scenes/Details/5
        public ActionResult Details(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBScene obj = db.NBScenes.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            BuildDetailsViewBag("Scenes", id, projectId, showHidden);

            return View(obj);
        }

        // GET: Scenes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            var obj = new NBScene();
            if (projectId != null && projectId > 0)
            {
                obj.NBProjectId = (int)projectId;
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Scenes");
            return View(obj);
        }

        // POST: Scenes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBSceneId,NBProjectId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBScene obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBScenes.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBSceneId, projectId = projectId, showHidden = showHidden });
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Scenes");
            return View(obj);
        }

        // GET: Scenes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBScene obj = db.NBScenes.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Scenes");
            return View(obj);
        }

        // POST: Scenes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBSceneId,NBProjectId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBScene obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBSceneId, projectId = projectId, showHidden = showHidden });
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Scenes");
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
