using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication_UESL.Models;

namespace WebApplication_UESL.Controllers
{
    public class CoursesController : Controller
    {
        private DatabaseProject_UESLEntities db = new DatabaseProject_UESLEntities();

        public ActionResult DeleteSelected(string[] ids)
        {
            //Delete Selected 
            int[] id = null;
            if (ids != null)
            {
                id = new int[ids.Length];
                int j = 0;
                foreach (string i in ids)
                {
                    int.TryParse(i, out id[j++]);
                }
            }

            if (id != null && id.Length > 0)
            {
                List<Course> allSelected = new List<Course>();

                allSelected = db.Courses.Where(a => id.Contains(a.ClassRecordID)).ToList();
                foreach (var h in allSelected)
                {
                    db.Courses.Remove(h);
                }
                db.SaveChanges();

            }
            return RedirectToAction("index");
        }

        // GET: Courses
        public ActionResult Index(string ClassName, string classID, string Instructor, string quarter)
        {
            var results = from r in db.Courses
                          select r;

            if (!String.IsNullOrEmpty(ClassName))
            {

                results = results.Where(s => s.ClassName.Contains(ClassName));
            }
            if (!String.IsNullOrEmpty(classID))
            {

                results = results.Where(s => s.classID.Contains(classID));
            }
            if (!String.IsNullOrEmpty(Instructor))
            {

                results = results.Where(s => s.Instructor.Contains(Instructor));
            }
            if (!String.IsNullOrEmpty(quarter))
            {

                results = results.Where(s => s.quarter.Contains(quarter));
            }

            return View(results);
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassRecordID,CatalogNumber,ClassName,Instructor,course_Name,MeetTime,Location,classID,quarter")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassRecordID,CatalogNumber,ClassName,Instructor,course_Name,MeetTime,Location,classID,quarter")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
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
