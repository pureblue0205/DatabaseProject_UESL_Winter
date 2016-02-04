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
    public class EnrollmentsController : Controller
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
                List<Enrollment> allSelected = new List<Enrollment>();

                allSelected = db.Enrollments.Where(a => id.Contains(a.EnrollmentID)).ToList();
                foreach (var h in allSelected)
                {
                    db.Enrollments.Remove(h);
                }
                db.SaveChanges();

            }
            return RedirectToAction("index");
        }


        // GET: Enrollments
        public ActionResult Index(string fName, string lName, string sID, string ClassName, string classID, string Grade, string Instructor, string quarter)
        {
            var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student);

            if (!String.IsNullOrEmpty(fName))
            {
                enrollments = enrollments.Where(s => s.Student.FirstName.Contains(fName));
            }
            if (!String.IsNullOrEmpty(lName))
            {
                enrollments = enrollments.Where(s => s.Student.LastName.Contains(lName));
            }
            if (!String.IsNullOrEmpty(sID))
            {
                int intSid = int.Parse(sID);
                enrollments = enrollments.Where(s => s.Student.StudentID == intSid);
            }
            if (!String.IsNullOrEmpty(ClassName))
            {
                enrollments = enrollments.Where(s => s.Course.ClassName.Contains(ClassName));
            }
            if (!String.IsNullOrEmpty(classID))
            {
                enrollments = enrollments.Where(s => s.Course.classID.Contains(classID));
            }
            if (!String.IsNullOrEmpty(Instructor))
            {
                enrollments = enrollments.Where(s => s.Course.Instructor.Contains(Instructor));
            }
            if (!String.IsNullOrEmpty(quarter))
            {
                enrollments = enrollments.Where(s => s.Course.quarter.Contains(quarter));
            }
            if (!String.IsNullOrEmpty(Grade))
            {
                enrollments = enrollments.Where(s => s.grade.Contains(Grade));
            }
            return View(enrollments.ToList());
        }

        // GET: Enrollments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollments/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "ClassRecordID", "classID");
            ViewBag.StudentID = new SelectList(db.Students, "UniqueRecordID", "StudentID");

                     
            return View();
        }



        // POST: Enrollments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EnrollmentID,CourseID,StudentID,grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "ClassRecordID", "classID", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "UniqueRecordID", "StudentID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ClassRecordID", "classID", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "UniqueRecordID", "StudentID", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EnrollmentID,CourseID,StudentID,grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ClassRecordID", "classID", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "UniqueRecordID", "StudentID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
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
