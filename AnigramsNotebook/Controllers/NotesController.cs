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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AnigramsNotebook.Controllers
{
    public class NotesController : BaseController
    {
        const string subscriptionKey = "mykey";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/recognizeText";

        // GET: Notes/Details/5
        public ActionResult Details(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBNote obj = db.NBNotes.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrentCategory = db.NBCategories.Find(obj.ObjectCategoryId);
            return View(obj);
        }

        // GET: Notes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? objectCategoryId, int? objectId, int? projectId, bool? showHidden = false)
        {
            var obj = new NBNote();
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

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "NBNoteId,NBProjectId,ObjectCategoryId,ObjectId,Name,Description,CreatedOn,LastModifiedOn")] NBNote obj, HttpPostedFileBase file, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ObjectId);
            if (ModelState.IsValid)
            {
                obj.CreatedOn = DateTime.Now;
                obj.LastModifiedOn = DateTime.Now;
                obj.IsActive = true;

                if (parent != null)
                {
                    obj.ObjectCategoryId = parent.NBCategoryId;
                    obj.ObjectId = parent.ObjectId;
                    UpdateParentObject(parent.NBCategoryId, parent.ObjectId);
                }

                if (file != null)
                {
                    string fileName = file.FileName;
                    string ext = fileName.Substring(fileName.LastIndexOf('.'));

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

                    var imageFilePath = Path.Combine(Server.MapPath("~/Content/images"),
                                        Path.GetFileName(string.Format("{0}.{1}", fileName, i.ResultFileExtension)));

                    var image = new NBImage()
                    {
                        CreatedOn = DateTime.Now,
                        Filename = string.Format("{0}.{1}", fileName, i.ResultFileExtension),
                        IsActive = true,
                        LastModifiedOn = DateTime.Now,
                        Name = file.FileName.Substring(0, file.FileName.Length - ext.Length),
                        NBProjectId = obj.NBProjectId,
                        ObjectCategoryId = obj.ObjectCategoryId,
                        ObjectId = obj.ObjectId
                    };
                    db.NBImages.Add(image);
                }
                db.NBNotes.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Details", "Notes", new { id = obj.NBNoteId, projectId = projectId, showHidden = showHidden });
            }

            BuildObjectsDropdown(projectId, showHidden, obj.ObjectCategoryId, obj.ObjectId);
            BuildProjectDropdown(projectId, showHidden, obj.NBProjectId);
            return View(obj);
        }

        // GET: Notes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, int? projectId, bool? showHidden = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NBNote obj = db.NBNotes.Find(id);
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

        // POST: Notes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "NBNoteId,NBProjectId,ObjectCategoryId,ObjectId,Name,Description,CreatedOn,LastModifiedOn, IsActive")] NBNote obj, int? projectId, bool? showHidden = false)
        {
            var parent = db.NBChanges_View.FirstOrDefault(x => x.NBChangeId == obj.ObjectId);
            if (ModelState.IsValid)
            {
                if (parent != null)
                {
                    obj.ObjectCategoryId = parent.NBCategoryId;
                    obj.ObjectId = parent.ObjectId;
                    UpdateParentObject(parent.NBCategoryId, parent.ObjectId);
                }
                obj.LastModifiedOn = DateTime.Now;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Notes", new { id = obj.NBNoteId, projectId = projectId, showHidden = showHidden });
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

        /// <summary>
        /// Gets the handwritten text from the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with handwritten text.</param>
        private async Task<string> ReadHandwrittenText(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameter. Set "handwriting" to false for printed text.
            string requestParameters = "handwriting=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response = null;

            // This operation requrires two REST API calls. One to submit the image for processing,
            // the other to retrieve the text found in the image. This value stores the REST API
            // location to call to retrieve the text.
            string operationLocation = null;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            ByteArrayContent content = new ByteArrayContent(byteData);

            // This example uses content type "application/octet-stream".
            // You can also use "application/json" and specify an image URL.
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // The first REST call starts the async process to analyze the written text in the image.
            response = await client.PostAsync(uri, content);

            // The response contains the URI to retrieve the result of the process.
            if (response.IsSuccessStatusCode)
                operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
            else
            {
                return JsonPrettyPrint(await response.Content.ReadAsStringAsync());
            }

            // The second REST call retrieves the text written in the image.
            //
            // Note: The response may not be immediately available. Handwriting recognition is an
            // async operation that can take a variable amount of time depending on the length
            // of the handwritten text. You may need to wait or retry this operation.
            //
            // This example checks once per second for ten seconds.
            string contentString;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(1000);
                response = await client.GetAsync(operationLocation);
                contentString = await response.Content.ReadAsStringAsync();
                ++i;
            }
            while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

            if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
            {
                return "Timeout error";
            }

            return JsonPrettyPrint(contentString);
        }


        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }


        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}
