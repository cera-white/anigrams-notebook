using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AnigramsNotebook.EF;
using ImageResizer;

namespace AnigramsNotebook.Controllers
{
    public class ImagesController : BaseController
    {
        // GET: Images/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? objectCategoryId, int? objectId, int? projectId, bool? showHidden = false)
        {
            var obj = new NBImage();
            if (objectCategoryId != null)
            {
                obj.ObjectCategoryId = (int)objectCategoryId;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (projectId != null && projectId > 0)
            {
                obj.NBProjectId = (int)projectId;
            }
            if (objectCategoryId != null && objectId != null)
            {
                var parent = db.NBChanges_View.FirstOrDefault(x => x.NBCategoryId == objectCategoryId && x.ObjectId == objectId);
                obj.ObjectId = (int)parent.NBChangeId;
                obj.NBProjectId = parent.NBProjectId;
            }
            
            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
            return View(obj);
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBImageId,NBProjectId,ObjectCategoryId,ObjectId,Name,Filename,CreatedOn,LastModifiedOn")] NBImage obj, HttpPostedFileBase[] files, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ObjectId);
            var generalObj = obj;
            if (ModelState.IsValid)
            {
                #region FileUpload
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var currentObj = new NBImage()
                        {
                            CreatedOn = DateTime.Now,
                            IsActive = true,
                            LastModifiedOn = DateTime.Now,
                            NBProjectId = obj.NBProjectId,
                            ObjectCategoryId = obj.ObjectCategoryId,
                            ObjectId = obj.ObjectId
                        };

                        string fileName = file.FileName;

                        string ext = fileName.Substring(fileName.LastIndexOf('.'));
                        currentObj.Name = fileName.Substring(0, fileName.Length - ext.Length);

                        if (file.ContentLength < 1 && !file.ContentType.Contains("image"))
                        {
                            ModelState.AddModelError("Filename", "Only image files are allowed.");

                            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
                            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
                            return View(obj);
                        }

                        fileName = Guid.NewGuid().ToString();

                        ImageJob i = new ImageJob(file, Server.MapPath("~/Content/images/" + fileName) + ".<ext>", new Instructions("width=2000;height=2000;format=jpg;mode=max"));
                        i.Build();

                        currentObj.Filename = string.Format("{0}.{1}", fileName, i.ResultFileExtension);

                        if (parent != null)
                        {
                            currentObj.ObjectCategoryId = parent.NBCategoryId;
                            currentObj.ObjectId = parent.ObjectId;
                            UpdateParentObject(parent.NBCategoryId, parent.ObjectId);
                        }

                        db.NBImages.Add(currentObj);
                    }
                }//end foreach
                #endregion
                db.SaveChanges();
                var category = db.NBCategories.Find(obj.ObjectCategoryId);
                if (parent != null)
                {
                    return RedirectToAction("Details", category.CategoryName, new { id = parent.ObjectId, projectId = projectId, showHidden = showHidden });
                }
                else
                {
                    return RedirectToAction("Index", category.CategoryName, new { projectId = projectId, showHidden = showHidden });
                }
            }

            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
            return View(obj);
        }

        // GET: Images/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBImage obj = db.NBImages.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBCategoryId == obj.ObjectCategoryId && x.ObjectId == obj.ObjectId);
            if (parent != null)
            {
                obj.ObjectId = (int)parent.NBChangeId;
            }
            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
            return View(obj);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBImageId,NBProjectId,ObjectCategoryId,ObjectId,Name,Filename,CreatedOn,LastModifiedOn,IsActive")] NBImage obj, HttpPostedFileBase files, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ObjectId);
            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    if (files.ContentLength < 1 && !files.ContentType.Contains("image"))
                    {
                        ModelState.AddModelError("Filename", "Only image files are allowed.");

                        BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
                        BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
                        return View(obj);
                    }

                    ImageJob i = new ImageJob(files, string.Format("~/Content/images/{0}", obj.Filename), new Instructions("width=2000;height=2000;format=jpg;mode=max"));
                    i.Build();
                }
                if (parent != null)
                {
                    obj.ObjectCategoryId = parent.NBCategoryId;
                    obj.ObjectId = parent.ObjectId;
                    UpdateParentObject(parent.NBCategoryId, parent.ObjectId);
                }
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                var category = db.NBCategories.Find(obj.ObjectCategoryId);
                if (parent != null)
                {
                    return RedirectToAction("Details", category.CategoryName, new { id = parent.ObjectId, projectId = projectId, showHidden = showHidden });
                }
                else
                {
                    return RedirectToAction("Index", category.CategoryName, new { projectId = projectId, showHidden = showHidden });
                }
            }
            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
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
