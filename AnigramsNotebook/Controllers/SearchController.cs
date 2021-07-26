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
    public class SearchController : Controller
    {
        private NotebookEntities db = new NotebookEntities();

        // GET: Search
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var objects = db.NBSearch_View.ToList();
            if (projectId != null && projectId > 0)
            {
                objects = objects.Where(x => x.NBProjectId == projectId || x.NBProjectId == 0).ToList();
            }
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true).ToList();
            }
            return View(objects.OrderBy(x => x.Name));
        }

        //POST: Search
        [HttpPost]
        public ActionResult Index(string query, int? projectId, bool? showHidden = false)
        {
            var objects = db.NBSearch_View.ToList();
            if (projectId != null && projectId > 0)
            {
                objects = objects.Where(x => x.NBProjectId == projectId || x.NBProjectId == 0).ToList();
            }
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true).ToList();
            }
            var objectsWithName = objects.Where(x => x.Name.Contains(query)).OrderBy(x => x.Name);
            var objectsWithDesc = objects.Where(x => x.Description.Contains(query)).OrderBy(x => x.Name);
            objects = objectsWithName.Union(objectsWithDesc).ToList();
            ViewBag.Query = query;
            return View(objects);
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
