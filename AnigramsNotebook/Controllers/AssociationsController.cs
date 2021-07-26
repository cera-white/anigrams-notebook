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
    public class AssociationsController : Controller
    {
        private NotebookEntities db = new NotebookEntities();

        // GET: Associations/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? parentCategoryId, int? parentId, int? projectId, bool? showHidden = false)
        {
            var obj = new NBAssociation();
            if (parentCategoryId != null && parentId != null)
            {
                obj.ParentId = (int)db.NBChanges_View.FirstOrDefault(x => x.NBCategoryId == parentCategoryId && x.ObjectId == parentId).NBChangeId;
            }
            BuildObjectsDropdown(projectId, showHidden, obj.ParentId, obj.ParentId);
            return View(obj);
        }

        // POST: Associations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBAssociationId,ParentCategoryId,ParentId,ChildCategoryId,ChildId,CreatedOn,LastModifiedOn")] NBAssociation obj, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ParentId);
            var child = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ChildId);

            if (ModelState.IsValid)
            {
                var existingObj = db.NBAssociations.FirstOrDefault(x => x.ParentCategoryId == parent.NBCategoryId && x.ParentId == parent.ObjectId && x.ChildCategoryId == child.NBCategoryId && x.ChildId == child.ObjectId);
                if (existingObj != null)
                {
                    // Association already exists - update it instead of creating a new one
                    obj = existingObj;
                    obj.LastModifiedOn = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    obj.ParentCategoryId = parent.NBCategoryId;
                    obj.ParentId = parent.ObjectId;
                    obj.ChildCategoryId = child.NBCategoryId;
                    obj.ChildId = child.ObjectId;
                    obj.ChildName = child.Name;
                    obj.IsActive = true;
                    obj.CreatedOn = DateTime.Now;
                    obj.LastModifiedOn = DateTime.Now;
                    db.NBAssociations.Add(obj);
                }
                
                db.SaveChanges();
                var category = db.NBCategories.Find(parent.NBCategoryId);
                return RedirectToAction("Details", category.CategoryName, new { id = parent.ObjectId, projectId = projectId, showHidden = showHidden });
            }
            BuildObjectsDropdown(projectId, showHidden, (int)parent.NBChangeId, (int)child.NBChangeId);
            return View(obj);
        }

        // GET: Associations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBAssociation obj = db.NBAssociations.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            var parentId = (int)db.NBChanges_View.FirstOrDefault(x => x.NBCategoryId == obj.ParentCategoryId && x.ObjectId == obj.ParentId).NBChangeId;
            var childId = (int)db.NBChanges_View.FirstOrDefault(x => x.NBCategoryId == obj.ChildCategoryId && x.ObjectId == obj.ChildId).NBChangeId;
            BuildObjectsDropdown(projectId, showHidden, parentId, childId);
            return View(obj);
        }

        // POST: Associations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBAssociationId,ParentCategoryId,ParentId,ChildCategoryId,ChildId,CreatedOn,LastModifiedOn,IsActive")] NBAssociation obj, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ParentId);
            var child = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ChildId);

            if (ModelState.IsValid)
            {
                obj.ParentCategoryId = parent.NBCategoryId;
                obj.ParentId = parent.ObjectId;
                obj.ChildCategoryId = child.NBCategoryId;
                obj.ChildId = child.ObjectId;
                obj.ChildName = child.Name;
                obj.LastModifiedOn = DateTime.Now;

                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                var category = db.NBCategories.Find(parent.NBCategoryId);
                return RedirectToAction("Details", category.CategoryName, new { id = parent.ObjectId, projectId = projectId, showHidden = showHidden });
            }
            BuildObjectsDropdown(projectId, showHidden, (int)parent.NBChangeId, (int)child.NBChangeId);
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

        private void BuildObjectsDropdown(int? projectId, bool? showHidden, int parentId, int childId)
        {
            var objects = db.NBChanges_View.ToList();
            if (projectId != null && projectId > 0)
            {
                objects = objects.Where(x => x.NBProjectId == projectId).ToList();
            }
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.ParentId = new SelectList(objects.OrderBy(x => x.Name), "NBChangeId", "Name", parentId);
            ViewBag.ChildId = new SelectList(objects.OrderBy(x => x.Name), "NBChangeId", "Name", childId);
        }
    }
}
