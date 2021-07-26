using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AnigramsNotebook.EF;

namespace AnigramsNotebook.Controllers
{
    public class BaseController : Controller
    {
        protected NotebookEntities db = new NotebookEntities();

        //// GET: Base
        //public ActionResult Index()
        //{
        //    return View();
        //}

        protected NBCategory BuildCurrentCategory(string categoryName)
        {
            var currentCategory = db.NBCategories.FirstOrDefault(x => x.CategoryName == categoryName);
            ViewBag.CurrentCategory = currentCategory;

            return currentCategory;
        }

        protected void BuildIndexViewBag(string categoryName, int? projectId, bool? showHidden)
        {
            var currentCategory = BuildCurrentCategory(categoryName);

            var updates = db.NBChanges_View.Where(x => x.CategoryName == currentCategory.CategoryName);
            var prompts = db.NBPrompts.Where(x => x.NBCategoryId == currentCategory.NBCategoryId);
            var notes = db.NBNotes.Where(x => x.NBCategory.NBCategoryId == currentCategory.NBCategoryId);
            var images = db.NBImages.Where(x => x.NBCategory.NBCategoryId == currentCategory.NBCategoryId);

            if (showHidden == false)
            {
                updates = updates.Where(x => x.IsActive == true);
                prompts = prompts.Where(x => x.IsActive == true);
                notes = notes.Where(x => x.IsActive == true);
                images = images.Where(x => x.IsActive == true);
            }

            if (projectId != null && projectId > 0)
            {
                updates = updates.Where(x => x.NBProjectId == projectId || x.NBProjectId == 0);
                notes = notes.Where(x => x.NBProjectId == projectId);
                images = images.Where(x => x.NBProjectId == projectId);
            }

            ViewBag.Updates = updates.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.Prompts = prompts.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.Notes = notes.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.Images = images.OrderByDescending(x => x.LastModifiedOn).ToList();
        }

        protected void BuildDetailsViewBag(string categoryName, int? id, int? projectId, bool? showHidden)
        {
            var currentCategory = BuildCurrentCategory(categoryName);

            var notes = db.NBNotes.Where(x => x.NBCategory.NBCategoryId == currentCategory.NBCategoryId && x.ObjectId == id);
            var images = db.NBImages.Where(x => x.NBCategory.NBCategoryId == currentCategory.NBCategoryId && x.ObjectId == id);
            var related = db.NBAssociations.Where(x => x.ParentCategoryId == currentCategory.NBCategoryId && x.ParentId == id);

            if (showHidden == false)
            {
                notes = notes.Where(x => x.IsActive == true);
                images = images.Where(x => x.IsActive == true);
                related = related.Where(x => x.IsActive == true);
            }

            if (projectId != null && projectId > 0)
            {
                notes = notes.Where(x => x.NBProjectId == projectId);
                images = images.Where(x => x.NBProjectId == projectId);
            }

            ViewBag.Notes = notes.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.Images = images.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.Related = related.OrderByDescending(x => x.LastModifiedOn).ToList();
            ViewBag.ObjectId = id;
        }

        protected void BuildProjectDropdown(int? projectId, bool? showHidden)
        {
            var projects = db.NBProjects.ToList();
            if (showHidden == false)
            {
                projects = projects.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.NBProjectId = new SelectList(projects.OrderBy(x => x.Name), "NBProjectId", "Name", projectId);
        }

        protected void BuildProjectDropdown(int? projectId, bool? showHidden, int? actualProjectId)
        {
            var projects = db.NBProjects.ToList();
            if (showHidden == false)
            {
                projects = projects.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.NBProjectId = new SelectList(projects.OrderBy(x => x.Name), "NBProjectId", "Name", actualProjectId);
        }

        protected void BuildUniverseDropdown(int? universeId, bool? showHidden)
        {
            var universes = db.NBUniverses.ToList();
            if (showHidden == false)
            {
                universes = universes.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.NBUniverseId = new SelectList(universes.OrderBy(x => x.Name), "NBUniverseId", "Name", universeId);
        }

        protected void BuildCategoryDropdown(int? projectId, bool? showHidden, int? categoryId)
        {
            var categories = db.NBCategories.ToList();
            if (showHidden == false)
            {
                categories = categories.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.NBCategoryId = new SelectList(categories, "NBCategoryId", "CategoryName", categoryId);
        }

        protected void BuildObjectsDropdown(int? projectId, bool? showHidden, int? objectCategoryId, int? objectId)
        {
            var objects = db.NBChanges_View.ToList();
            if (objectCategoryId != null && objectCategoryId > 0)
            {
                objects = objects.Where(x => x.NBCategoryId == objectCategoryId).ToList();
            }
            var categoriesToSkip = db.NBCategories.Where(x => x.CategoryName == "Universes" || x.CategoryName == "Projects");
            if (projectId != null && projectId > 0 && !categoriesToSkip.Any(x => x.NBCategoryId == objectCategoryId))
            {
                objects = objects.Where(x => x.NBProjectId == projectId).ToList();
            }
            if (showHidden == false)
            {
                objects = objects.Where(x => x.IsActive == true).ToList();
            }
            ViewBag.ObjectId = new SelectList(objects.OrderBy(x => x.Name), "NBChangeId", "Name", objectId);
        }

        protected void UpdateParentObject(int parentCategoryId, int parentId)
        {
            var category = db.NBCategories.Find(parentCategoryId);

            var pkSql = string.Format(@"SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
            WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1
            AND TABLE_NAME = '{0}' AND TABLE_SCHEMA = 'dbo'", category.TableName);
            var primaryKey = db.Database.SqlQuery<string>(pkSql).Single();

            var updateSql = string.Format("UPDATE {0} SET LastModifiedOn = SYSDATETIME() WHERE {1} = {2}", category.TableName, primaryKey, parentId);

            db.Database.ExecuteSqlCommand(updateSql);
        }
    }
}