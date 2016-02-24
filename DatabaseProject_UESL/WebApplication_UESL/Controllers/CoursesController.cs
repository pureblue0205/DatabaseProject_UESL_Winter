using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication_UESL.Models;
using WebApplication_UESL.helpers;

using OfficeOpenXml;
using System.Xml;

using OfficeOpenXml.Style;

namespace WebApplication_UESL.Controllers
{
    public class CoursesController : Controller
    {
        private DatabaseProject_UESLEntities db = new DatabaseProject_UESLEntities();

        public void exportButtonClick(int[] idsToExport)
        {
            var fileDownloadName = "Courses.xlsx";

            List<Course> courses = new List<Course>();
            if (idsToExport != null && idsToExport.Length != 0)
            {
                courses = db.Courses.Where(a => idsToExport.Contains(a.ClassRecordID)).ToList();
            }

            ExcelExport filemaker = new ExcelExport();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDownloadName);
            Response.BinaryWrite(filemaker.exportCourses(courses));
            Response.Flush();
            Response.Close();

            
        }

        public ActionResult export(string[] ids2, string submitButton)
        {
            //first process the ids and make them ints 
            int[] id = null;
            if (ids2 != null)
            {
                id = new int[ids2.Length];
                int j = 0;
                foreach (string i in ids2)
                {
                    int.TryParse(i, out id[j++]);
                }
            }
            //remove from exportkeys table
            if (submitButton.Equals("remove"))
            {
                if (id != null && id.Length > 0)
                {
                    List<courseExportKey> allSelected = new List<courseExportKey>();

                    allSelected = db.courseExportKeys.Where(a => id.Contains(a.courseKey)).ToList();

                    foreach (var i in allSelected)
                    {
                        db.courseExportKeys.Remove(i);
                    }
                    db.SaveChanges();
                }
            }//end "remove" submit button

            if (submitButton.Equals("exp"))
            {
                //call the exporting class!
                //sending the id array of courseids of students to export
                exportButtonClick(id);
            }
            return RedirectToAction("index");
        }

        public PartialViewResult coursesExport()
        {
            List<courseExportKey> keys = new List<courseExportKey>();
            keys = db.courseExportKeys.ToList();

            List<Course> results = new List<Course>();

            foreach (courseExportKey k in keys)
            {
                results.Add((db.Courses.Find(k.courseKey)));
            }

            return PartialView(results);
        }

        public ActionResult registerSelected(string[] ids, string classRecID)
        {

            string cRecID = "";

            if (String.IsNullOrEmpty(classRecID))
            {
                cRecID = Request.QueryString["uid"];

                ViewData["classRecID"] = cRecID;

            }
            if (!(String.IsNullOrEmpty(classRecID)))
            {
                cRecID = classRecID;
                ViewData["classRecID"] = cRecID;

            }
            int courseKey = int.Parse(cRecID);

            List<Student> studentsToAdd = new List<Student>();

            if (ids != null && ids.Length != 0)
            {
                foreach (string s in ids)
                {
                    int sint = int.Parse(s);
                    //this section makes sure you cant create an identical enrollment twice
                    var checker = from r in db.Enrollments
                                  select r;
                    checker = checker.Where(e => e.CourseID == courseKey && e.Student.UniqueRecordID == sint);
                    if (!(checker.Any()))
                    {
                        // no Match!
                        studentsToAdd.Add(db.Students.Find(sint));
                    }                                       
                }
            }

            
            Course courseToUse = db.Courses.Find(courseKey);

            foreach (Student k in studentsToAdd)
            {
                Enrollment e = new Enrollment();
                e.CourseID = courseToUse.ClassRecordID;
                e.StudentID = k.UniqueRecordID;
                db.Enrollments.Add(e);
                db.SaveChanges();
            }

            string toRedirect = "details/" + cRecID;
            return RedirectToAction(toRedirect);

        }

        public ActionResult DeleteEnrollment(int id, int cRecID)
        {

            int[] idsArr = new int[1];
            idsArr[0] = id;

            //first delete the export key for this enrollment, if it exists
            List<enrollmentExportKey> allSelectedKeys = new List<enrollmentExportKey>();
            allSelectedKeys = db.enrollmentExportKeys.Where(a => idsArr.Contains(a.enrollmentKey)).ToList();

            foreach (var ek in allSelectedKeys)
            {
                db.enrollmentExportKeys.Remove(ek);
            }


            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            string toRedirect = "details/" + cRecID;
            return RedirectToAction(toRedirect);
        }

        public ActionResult addEnrollment(string firstName, string lastName, string sID, string Citezenship, string agent, string I20exp, string classRecID)
        {

            string cRecID;

            if (String.IsNullOrEmpty(classRecID))
            {
                cRecID = Request.QueryString["uid"];                

                ViewData["classRecID"] = cRecID;
                
            }
            if (!(String.IsNullOrEmpty(classRecID)))
            {
                cRecID = classRecID;
                ViewData["classRecID"] = cRecID;

            }



            var results = from s in db.Students
                          select s;

            if (!String.IsNullOrEmpty(firstName))
            {

                results = results.Where(s => s.FirstName.Contains(firstName));
            }
            if (!String.IsNullOrEmpty(lastName))
            {

                results = results.Where(s => s.LastName.Contains(lastName));
            }
            if (!String.IsNullOrEmpty(sID))
            {
                int sInt = int.Parse(sID);
                results = results.Where(s => s.StudentID == sInt);
            }
            if (!String.IsNullOrEmpty(Citezenship))
            {

                results = results.Where(s => s.Citezenship.Contains(Citezenship));
            }
            if (!String.IsNullOrEmpty(agent))
            {

                results = results.Where(s => s.School_Agent.Contains(agent));
            }

            return View(results);
        }

        public PartialViewResult enrolledstudents()
        {
            var results = from c in db.Enrollments
                          select c;

            int cRecID = (int)TempData["classRecID"];

            results = results.Where(c => c.Course.ClassRecordID == cRecID);

            return PartialView(results);


        }

        //this method also handles marking for export!
        //dont let the name fool you
        //if the mark for export button is clicked, "submitButton" will be "ex"
        //if the delete selected button is clicked, "submitButton" will be "del"
        public ActionResult DeleteSelected(string[] ids, string submitButton)
        {

            //first process the ids and make them ints 
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

            //delete selected part
            if (submitButton.Equals("del"))
            {
                if (id != null && id.Length > 0)
                {
                    //first i must check and make to sure delete all export keys that refer to the students that i am deleting!
                    //otherwise everything breaks!
                    //do that here now

                    List<courseExportKey> allSelectedKeys = new List<courseExportKey>();
                    allSelectedKeys = db.courseExportKeys.Where(a => id.Contains(a.courseKey)).ToList();

                    foreach (var sk in allSelectedKeys)
                    {
                        db.courseExportKeys.Remove(sk);
                    }

                    //delete the course records
                    List<Course> allSelected = new List<Course>();

                    allSelected = db.Courses.Where(a => id.Contains(a.ClassRecordID)).ToList();
                    foreach (var h in allSelected)
                    {
                        db.Courses.Remove(h);
                    }
                    db.SaveChanges();

                }
            }//end delete

            //mark for export part
            if (submitButton.Equals("ex"))
            {
                
                List<int> all = new List<int>();
                var temp = db.courseExportKeys.ToList();
                foreach (courseExportKey k in temp)
                {
                    all.Add(k.courseKey);
                }

                if (id != null && id.Length > 0)
                {
                    foreach (int i in id)
                    {
                        int alreadyExists = 0;
                        foreach (int sk in all)
                        {

                            //this makes sure there wont be duplicates when exporting
                            if (sk == i)
                            {
                                alreadyExists = 1;

                            }

                        }

                        if (alreadyExists != 1)
                        {
                            //put the new export key in the database
                            courseExportKey k = new courseExportKey();

                            k.courseKey = i;
                            db.courseExportKeys.Add(k);
                        }

                    }//end foreach
                    db.SaveChanges();
                }
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

            TempData["classRecID"] = course.ClassRecordID;

            
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
