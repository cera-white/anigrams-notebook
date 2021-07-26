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
    public class CharactersController : BaseController
    {
        // GET: Characters
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBCharacters.Include(n => n.NBProject);
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true);
            }

            if (projectId != null && projectId > 0)
            {
                objects = objects.Where(x => x.NBProjectId == projectId);
            }

            BuildIndexViewBag("Characters", projectId, showHidden);

            return View(objects.OrderByDescending(x => x.LastModifiedOn).ToList());
        }

        // GET: Characters/Details/5
        public ActionResult Details(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBCharacter obj = db.NBCharacters.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            BuildDetailsViewBag("Characters", id, projectId, showHidden);

            return View(obj);
        }

        // GET: Characters/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            var obj = new NBCharacter();
            if (projectId != null && projectId > 0)
            {
                obj.NBProjectId = (int)projectId;
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Characters");
            return View(obj);
        }

        // POST: Characters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBCharacterId,NBProjectId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBCharacter obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBCharacters.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBCharacterId, projectId = projectId, showHidden = showHidden });
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Characters");
            return View(obj);
        }

        // GET: Characters/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBCharacter obj = db.NBCharacters.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Characters");
            return View(obj);
        }

        // POST: Characters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBCharacterId,NBProjectId,Name,Description,IsActive,CreatedOn,LastModifiedOn")] NBCharacter obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = obj.NBCharacterId, projectId = projectId, showHidden = showHidden });
            }
            BuildProjectDropdown(obj.NBProjectId, showHidden);
            var currentCategory = BuildCurrentCategory("Characters");
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
