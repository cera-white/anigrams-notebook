using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AnigramsNotebook.EF;
using System.Data.Entity;
using System.Web.Helpers;

namespace AnigramsNotebook
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new MyPropertyActionFilter(), 0);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public class MyPropertyActionFilter : ActionFilterAttribute
    {
        private NotebookEntities db = new NotebookEntities();

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

            var categories = db.NBCategories.Where(x => x.IsActive == true).OrderBy(x => x.Rank);
            filterContext.Controller.ViewBag.MenuOptions = categories;
            var projects = db.NBProjects.Where(x => x.IsActive == true);
            filterContext.Controller.ViewBag.Projects = projects;
            int projectId;
            int.TryParse(filterContext.HttpContext.Request.Unvalidated().QueryString["projectId"], out projectId);
            var selectedProject = projects.FirstOrDefault(x => x.NBProjectId == projectId);
            filterContext.Controller.ViewBag.ProjectId = projectId;
            filterContext.Controller.ViewBag.ProjectName = selectedProject == null ? "All Projects" : selectedProject.Name;
            bool showHidden;
            bool.TryParse(filterContext.HttpContext.Request.Unvalidated().QueryString["showHidden"], out showHidden);
            filterContext.Controller.ViewBag.ShowHidden = showHidden;

            foreach (var item in categories)
            {
                var whereClause = string.Format("WHERE {0} {1}", (showHidden ? "1 = 1" : "IsActive = 1"), (projectId > 0 && item.CategoryName != "Universes" && item.CategoryName != "Projects") ? string.Format("AND NBProjectId = {0}", projectId) : "");
                var sql = string.Format("SELECT COUNT(1) FROM {0} {1}", item.TableName, whereClause);
                var result = db.Database.SqlQuery<int>(sql).Single();
                filterContext.Controller.ViewData[item.CategoryName + "Count"] = result;
            }
        }
    }
}
