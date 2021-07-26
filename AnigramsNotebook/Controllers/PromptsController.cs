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
    public class PromptsController : BaseController
    {
        // GET: Prompts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? categoryId, int? projectId, bool? showHidden = false)
        {
            BuildCategoryDropdown(projectId, showHidden, categoryId);
            return View();
        }

        // POST: Prompts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBPromptId,NBCategoryId,Text,Source,IsActive,CreatedOn,LastModifiedOn")] NBPrompt obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBPrompts.Add(obj);
                db.SaveChanges();
                var category = db.NBCategories.Find(obj.NBCategoryId);
                return RedirectToAction("Index", category.CategoryName, new { projectId = projectId, showHidden = showHidden });
            }

            BuildCategoryDropdown(projectId, showHidden, obj.NBCategoryId);
            return View(obj);
        }

        // GET: Prompts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBPrompt obj = db.NBPrompts.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            BuildCategoryDropdown(projectId, showHidden, obj.NBCategoryId);
            return View(obj);
        }

        // POST: Prompts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBPromptId,NBCategoryId,Text,Source,IsActive,CreatedOn,LastModifiedOn")] NBPrompt obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                var category = db.NBCategories.Find(obj.NBCategoryId);
                return RedirectToAction("Index", category.CategoryName, new { projectId = projectId, showHidden = showHidden });
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
