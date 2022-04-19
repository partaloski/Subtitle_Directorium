using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Subtitle_Directorium.Models;

namespace Subtitle_Directorium.Controllers
{
    public class MovieModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        // GET: MovieModels
        public ActionResult Index()
        {
            return View(db.Movies);
        }

        [Authorize(Roles = "Admin,Editor,Submittor")]
        // GET: MovieModels/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View(db.Movies);
        }

        // POST: MovieModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Editor,Submittor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Rating,URL,Description,Genre")] MovieModel movieModel)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movieModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movieModel);
        }

        [Authorize(Roles = "Admin,Editor")]
        // GET: MovieModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        [Authorize(Roles = "Admin,Editor")]
        // POST: MovieModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Rating,URL,Description,Genre")] MovieModel movieModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movieModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/"+movieModel.ID);
            }
            return View(movieModel);
        }

        [Authorize(Roles = "Admin,Editor")]
        // GET: MovieModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            return View(movieModel);
        }

        [Authorize(Roles = "Admin,Editor")]
        // POST: MovieModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieModel movieModel = db.Movies.Find(id);
            db.Movies.Remove(movieModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        // GET: MovieModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MovieModel movieModel = db.Movies.Find(id);
            if (movieModel == null)
            {
                return HttpNotFound();
            }
            List<Subtitle> subs = new List<Subtitle>();
            foreach (Subtitle s in db.Subtitles)
            {
                if (s.movieId == null)
                    continue;
                else if (s.movieId == movieModel.ID)
                    subs.Add(s);
            }
            Session["URL"] = movieModel.URL;
            movieModel.movieSubtitles = subs;
            db.Entry(movieModel).State = EntityState.Modified;
            db.SaveChanges();
            return View(movieModel);
        }

        [Authorize(Roles = "Admin,Editor,Submittor")]
        public ActionResult AddSubtitle(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MovieModel movie = db.Movies.Find(id);
            if (movie == null)
            {
                return View();
            }
            Subtitle subtitle = new Subtitle();
            Session["Movie"] = movie;
            Session["MovieName"] = movie.Name;
            Session["MovieID"] = movie.ID;
            return View(subtitle);
        }
        [Authorize(Roles = "Admin,Editor,Submittor")]
        [HttpPost, ActionName("AddSubtitle")]
        [ValidateInput(false)]
        public ActionResult AddSubtitle(Subtitle subtitle)
        {
            if (ModelState.IsValid)
            {
                subtitle.movieId = (int)Session["MovieID"];
                subtitle.movieName = (string)Session["MovieName"];
                db.Subtitles.Add(subtitle);
                db.SaveChanges();
                Session["sub"] = subtitle;
                return RedirectToAction("CreateLines");
            }
            return View(subtitle);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,Submittor")]
        public ActionResult AddSubtitleFile(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MovieModel movie = db.Movies.Find(id);
            if (movie == null)
            {
                return View();
            }
            Subtitle subtitle = new Subtitle();
            Session["Movie"] = movie;
            Session["MovieName"] = movie.Name;
            Session["MovieID"] = movie.ID;
            return View(subtitle);
        }
        [Authorize(Roles = "Admin,Editor,Submittor")]
        [HttpPost, ActionName("AddSubtitleFile")]
        [ValidateInput(false)]
        public ActionResult AddSubtitleFile(Subtitle subtitle)
        {
            subtitle.movieId = (int)Session["MovieID"];
            subtitle.movieName = (string)Session["MovieName"];
            HttpPostedFileBase [] f =  (HttpPostedFileBase [])subtitle.SrtFile;
            Debug.WriteLine("There are # files ->"+f.Length);
            HttpPostedFileBase file = f[0];
            string FileName = Path.GetFileNameWithoutExtension(file.FileName);

            //To Get File Extension  
            string FileExtension = Path.GetExtension(file.FileName);

            //Add Current Date To Attached File Name  
            FileName = "tmp.srt";

            //Get Upload path from Web.Config file AppSettings.  
            string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

            string path = UploadPath + FileName;

            //To copy and save file into server.  
            file.SaveAs(path);

            if (subtitle.language != null)
            {
                subtitle.readFile();
                //File.Delete(Path.Combine(ConfigurationManager.AppSettings["UserImagePath"].ToString(), FileName));
                Debug.WriteLine(subtitle.raw.Length);
                Debug.WriteLine(subtitle.ID);
                Session["sub"] = subtitle;
                db.Subtitles.Add(subtitle);
                db.SaveChanges();
                return RedirectToAction("CreateLines");
            }
            return View(subtitle);
        }


        [Authorize(Roles = "Admin,Editor,Submittor")]
        public ActionResult CreateLines()
        {
            Subtitle subtitle = (Subtitle)Session["sub"];
            if (subtitle != null)
            {
                Debug.WriteLine(subtitle.ID);
                Debug.WriteLine("I am here, in CreateLines"); 
                subtitle.addLines(subtitle.ID, subtitle.raw);
                Debug.WriteLine(subtitle.lines.Length);
                foreach (Line line in subtitle.lines)
                {
                    db.Lines.Add(line);
                }
                db.Entry(subtitle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/" + subtitle.movieId);
            }
            else
            {
                Debug.WriteLine("The id was null!");
                return RedirectToAction("Index");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        [AllowAnonymous]
        // GET: Subtitles/Details/5
        public ActionResult SubDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subtitle subtitle = db.Subtitles.Find(id);
            if (subtitle == null)
            {
                return HttpNotFound();
            }
            List<Line> lines = new List<Line>();
            foreach (Line line in db.Lines)
            {
                if (line.subtitleID == id)
                    lines.Add(line);
            }
            Session["subLines"] = lines;
            Session["subID"] = id;
            return View(subtitle);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubEdit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Subtitle subtitle = db.Subtitles.Find(id);
            if (subtitle == null)
            {
                return RedirectToAction("Index");
            }
            Session["movieName"] = subtitle.movieName;
            Session["id"] = id;
            return View(subtitle);
        }

        // POST: Subtitles1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SubEdit([Bind(Include = "ID,language,raw")] Subtitle subtitle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subtitle).State = EntityState.Modified;
                subtitle.addLines((int) Session["id"], subtitle.raw);
                foreach (Line line in subtitle.lines)
                {
                    db.Lines.Add(line);
                }
                subtitle.movieName = (string)Session["movieName"];
                subtitle.movieId = (int)Session["MovieID"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subtitle);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubAddC(int ? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Subtitle subtitle = db.Subtitles.Find(id);
            if (subtitle == null)
            {
                return RedirectToAction("Index");
            }
            Subtitle s = new Subtitle();
            s.movieName = subtitle.movieName;
            s.language = subtitle.language;
            s.movieId = subtitle.movieId;
            s.raw = subtitle.raw;
            db.Subtitles.Add(s);
            db.SaveChanges();
            
            return RedirectToAction("SubEdit\\" + s.ID);
        }

        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public ActionResult SubAddC(Subtitle subtitle)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details\\" + subtitle.ID);
            }
            Subtitle newSubtitle = new Subtitle();
            newSubtitle.movieName = (string)Session["movieName"];
            newSubtitle.movieId = subtitle.movieId;
            newSubtitle.raw = subtitle.raw;
            newSubtitle.language = subtitle.language;
            db.Subtitles.Add(newSubtitle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubEditLine(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            int idd = (int)id;
            if (Session["lineNum"] == null)
            {
                Session["lineNum"] = 0;
            }
            int linenr = (int)Session["lineNum"];
            Subtitle subtitle = db.Subtitles.Find(id);
            subtitle.addLines(subtitle.ID, subtitle.raw);

            if (linenr > subtitle.lines.Count())
                linenr = 0;
            if (linenr < subtitle.lines.Count())
                linenr = (subtitle.lines.Count()) - 1;
            if (subtitle.lines.Count() == 0)
            {
                Debug.WriteLine("No lines?");
                return RedirectToAction("SubDetails/" + id);
            }
            Line line = subtitle.lines.ElementAt(linenr);
            Session["id"] = id;
            Session["lineNum"] = linenr;
            Session["subtitle"] = subtitle;
            return View(line);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubNextLine(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Session["lineNum"] = (int)Session["lineNum"] + 1;
            return View(id.ToString());
        }
        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public ActionResult SubNextLine(string id)
        {
            int idd = Convert.ToInt32(id);
            return RedirectToAction("SubEditLine/" + idd);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubPrevLine(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Session["lineNum"] = (int)Session["lineNum"] - 1;
            return View(id.ToString());
        }
        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public ActionResult SubPrevLine(string id)
        {
            int idd = Convert.ToInt32(id);
            return RedirectToAction("SubEditLine/" + idd);
        }

        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public ActionResult SubEditLine(Line l)
        {
            return View(l);
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult SubSaveLine (int ? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Subtitle subtitle = (Subtitle)Session["subtitle"];
            return View(subtitle);
        }

        [HttpPost]
        public ActionResult SubSaveLine(Subtitle subtitle)
        {
            db.Entry(subtitle).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Editor")]
        public ActionResult LineEdit(int ? id)
        {
            if(id == null)
            {
                return RedirectToAction("SubDetails/" + (int)Session["subID"]);
            }
            Line line = db.Lines.Find(id);
            Session["lineNumber"] = line.number;
            Session["timestamp"] = line.timestamp;
            Session["subID"] = line.subtitleID;
            if (line == null)
            {
                return RedirectToAction("SubDetails/" + (int)Session["subID"]);
            }
            return View(line);
        }
        [Authorize (Roles ="Admin,Editor")]
        [HttpPost]
        public ActionResult LineEdit(Line line)
        {
            if (ModelState.IsValid)
            {
                db.Entry(line).State = EntityState.Modified;
                line.subtitleID = (int)Session["subID"];
                line.number = (int)Session["lineNumber"];
                line.timestamp = (string)Session["timestamp"];
                List<Line> lines = new List<Line>();
                bool foundOne = false;
                foreach (Line linex in db.Lines)
                {
                    if (foundOne == true && linex.subtitleID != line.subtitleID)
                    {
                        break;
                    }
                    if (linex.subtitleID == line.subtitleID)
                    {
                        foundOne = true;
                        lines.Add(linex);
                    }
                }
                Subtitle subtitle = db.Subtitles.Find(line.subtitleID);
                subtitle.updateRaw(lines);
                db.Entry(subtitle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SubDetails/" + (int)Session["subID"]);
            }
            return View(line);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult DownloadSubtitle(int ? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            Subtitle subtitle = db.Subtitles.Find(id);
            if(subtitle == null)
            {
                return RedirectToAction("Index");
            }
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename="+subtitle.movieName+" "+subtitle.language+".srt");
            Response.ContentType = "text/csv";
            Response.Write(subtitle.raw);
            Response.End();
            return Content(String.Empty);
        }


        public ActionResult ChangeSort(int ? id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Session["sortFilter"] = (int)id;
            return RedirectToAction("Index");
        }
    }
    

}
