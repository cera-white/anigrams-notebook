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
    public class UniversesController : BaseController
    {
        // GET: Universes
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBUniverses.ToList();
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true).ToList();
            }

            BuildIndexViewBag("Universes", projectId, showHidden);

            return View(objects.OrderByDescending(x => x.LastModifiedOn));
        }

        // GET: Universes/Details/5
        public ActionResult Details(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBUniverse obj = db.NBUniverses.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            BuildDetailsViewBag("Universes", id, projectId, showHidden);

            return View(obj);
        }

        // GET: Universes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            var obj = new NBUniverse();
            var currentCategory = BuildCurrentCategory("Universes");
            return View(obj);
        }

        // POST: Universes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBUniverseId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBUniverse obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBUniverses.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBUniverseId, projectId = projectId, showHidden = showHidden });
            }

            var currentCategory = BuildCurrentCategory("Universes");
            return View(obj);
        }

        // GET: Universes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBUniverse obj = db.NBUniverses.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            var currentCategory = BuildCurrentCategory("Universes");
            return View(obj);
        }

        // POST: Universes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBUniverseId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBUniverse obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBUniverseId, projectId = projectId });
            }
            var currentCategory = BuildCurrentCategory("Universes");
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
