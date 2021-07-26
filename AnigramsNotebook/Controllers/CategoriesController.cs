using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AnigramsNotebook.EF;
using System.Data.SqlClient;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Diagnostics;

namespace AnigramsNotebook.Controllers
{
    public class CategoriesController : Controller
    {
        private NotebookEntities db = new NotebookEntities();
        private static CultureInfo ci = new CultureInfo("en-us");
        private PluralizationService ps =
          PluralizationService.CreateService(ci);

        // GET: Categories
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var categories = db.NBCategories.OrderBy(x => x.Rank).ToList();
            if (showHidden == false)
            {
                categories = categories.Where(x => x.IsActive == true).ToList();
            }
            return View(categories);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? projectId, bool? showHidden = false)
        {
            var obj = new NBCategory();
            obj.Rank = db.NBCategories.Max(x => x.Rank) + 1;
            return View(obj);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBCategoryId,CategoryName,TableName,IconName,Color,Rank,IsActive,CreatedOn,LastModifiedOn")] NBCategory obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.TableName = "NB" + obj.CategoryName;
                obj.IsActive = true;
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                db.NBCategories.Add(obj);
                db.SaveChanges();

                var tableNameParam = new SqlParameter("@TableName", obj.TableName);
                var primaryKeyParam = new SqlParameter("@PrimaryKey", ps.Singularize(obj.TableName) + "Id");
                var result = db.Database.ExecuteSqlCommand("USP_CreateCategory @TableName, @PrimaryKey", tableNameParam, primaryKeyParam);

                return RedirectToAction("Index", new { projectId = projectId, showHidden = showHidden });
            }

            return View(obj);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBCategory obj = db.NBCategories.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBCategoryId,CategoryName,TableName,IconName,Color,Rank,IsActive,CreatedOn,LastModifiedOn")] NBCategory obj, int? projectId, bool? showHidden = false)
        {
            if (ModelState.IsValid)
            {
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { projectId = projectId, showHidden = showHidden });
            }
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
