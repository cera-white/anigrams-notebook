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
    public class HomeController : Controller
    {
        private NotebookEntities db = new NotebookEntities();
        private Random rand = new Random();

        public ActionResult Index(int? projectId, bool? showHidden = false)
        {
            var prompts = db.NBPrompts.ToList();
            var bookmarks = db.NBResources.ToList();
            var updates = db.NBChanges_View.ToList();
            if (showHidden == false)
            {
                prompts = prompts.Where(x => x.IsActive == true).ToList();
                bookmarks = bookmarks.Where(x => x.IsActive == true).ToList();
                updates = updates.Where(x => x.IsActive == true).ToList();
            }
            if (projectId != null && projectId > 0)
            {
                updates = updates.Where(x => x.NBProjectId == projectId).ToList();
            }
            var selectedPrompt = prompts.ElementAt(rand.Next(prompts.Count()));
            ViewBag.Prompt = selectedPrompt;
            ViewBag.Updates = updates.OrderByDescending(x => x.LastModifiedOn).Take(4);
            ViewBag.Bookmarks = bookmarks.OrderByDescending(x => x.LastModifiedOn).Take(4);
            return View();
        }
    }
}