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
    public class UpdatesController : Controller
    {
        private NotebookEntities db = new NotebookEntities();

        // GET: Updates
        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var updates = db.NBChanges_View.ToList();
            if (projectId != null && projectId > 0)
            {
                updates = updates.Where(x => x.NBProjectId == projectId || x.NBProjectId == 0).ToList();
            }
            if (showHidden == false)
            {
                updates = updates.Where(x => x.IsActive == true).ToList();
            }
            return View(updates.OrderByDescending(x => x.LastModifiedOn));
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
