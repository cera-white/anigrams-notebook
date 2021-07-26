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
    public class ResourcesController : BaseController
    {
        // GET: Resources
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBResources.Include(n => n.NBCategory);
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true);
            }
            return View(objects.OrderByDescending(x => x.LastModifiedOn).ToList());
        }

        // GET: Resources/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            BuildCategoryDropdown(projectId, showHidden, 0);
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBResourceId,NBCategoryId,DisplayName,Url,IsActive,CreatedOn,LastModifiedOn")] NBResource obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBResources.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Index", "Resources", new { projectId = projectId, showHidden = showHidden });
            }

            BuildCategoryDropdown(projectId, showHidden, obj.NBCategoryId);
            return View(obj);
        }

        // GET: Resources/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBResource obj = db.NBResources.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            BuildCategoryDropdown(projectId, showHidden, obj.NBCategoryId);
            return View(obj);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBResourceId,NBCategoryId,DisplayName,Url,IsActive,CreatedOn,LastModifiedOn")] NBResource obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Resources", new { projectId = projectId, showHidden = showHidden });
            }
            BuildCategoryDropdown(projectId, showHidden, obj.NBCategoryId);
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
