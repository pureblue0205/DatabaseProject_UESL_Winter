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
    public class StudentsController : Controller
    {
        

        private DatabaseProject_UESLEntities db = new DatabaseProject_UESLEntities();

        
        
        public ActionResult DeleteEnrollment(int id, int sRecID)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            string toRedirect = "details/" + sRecID;
            return RedirectToAction(toRedirect);
        }

        //partial view that shows currently  enrolled classes for this particular student
        public PartialViewResult enrolledCourses()
        {
            var results = from r in db.Enrollments
                          select r;

            int sID = (int)TempData["studID"];

            //actually comparing "uniquerecordID" of student
            results = results.Where(s => s.StudentID == sID);

            return PartialView(results);
        }

        //constructor for addEnrollment view
        public ActionResult addEnrollment(string ClassName, string classID, string Instructor, string quarter, string studentRecordID, string stID)
        {

            string studentUID;
            string sID;

            if (String.IsNullOrEmpty(studentRecordID) && String.IsNullOrEmpty(stID))
            {
                studentUID = Request.QueryString["uid"];
                sID = Request.QueryString["studentID"];

                ViewData["studentRecordID"] = studentUID;
                ViewData["stID"] = sID;
            }

            if (!(String.IsNullOrEmpty(studentRecordID) && String.IsNullOrEmpty(stID)))
            {
                ViewData["studentRecordID"] = studentRecordID;
                ViewData["stID"] = stID;
            }

           




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
        //ids : the unique id for the course record
        public ActionResult registerSelected(string[] ids, string studentRecordID, string stID)
        {

            string studentUID = "";
            string sID = "";

            if (String.IsNullOrEmpty(studentRecordID) && String.IsNullOrEmpty(stID))
            {
                studentUID = Request.QueryString["uid"];
                sID = Request.QueryString["studentID"];

                ViewData["studentRecordID"] = studentUID;
                ViewData["stID"] = sID;
            }
            if (!(String.IsNullOrEmpty(studentRecordID) && String.IsNullOrEmpty(stID)))
            {
                studentUID = studentRecordID;
                sID = stID;

                ViewData["studentRecordID"] = studentUID;
                ViewData["stID"] = sID;

            }
            //todo: add enrolments for selected classes (in ids array)
            // Use UiquerecordID not student id for the ""StudentID" category of enrollments
            //first get a list of the courses that needed to be added - done
            //then make an enrollment for each course for that student
            
            //this gets all the courses that were selected
            List<Course> coursesToAdd = new List<Course>();
            if (ids != null && ids.Length != 0)
            {
                foreach (string s in ids)
                {
                   int sint = int.Parse(s);
                   coursesToAdd.Add(db.Courses.Find(sint));                  
                }
            }

            //gets the student in question
            int studentKey = int.Parse(studentUID);
            Student studentToUSe = db.Students.Find(studentKey);

            //now make the enrollments and add them to the DB
            foreach (Course k in coursesToAdd)
            {
                Enrollment e = new Enrollment();
                e.CourseID = k.ClassRecordID;
                e.StudentID = studentToUSe.UniqueRecordID;
                db.Enrollments.Add(e);
                db.SaveChanges();
            }

            string toRedirect = "details/" + studentRecordID;
            
            return RedirectToAction(toRedirect);
        }

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
                List<Student> allSelected = new List<Student>();
                
                    allSelected = db.Students.Where(a => id.Contains(a.UniqueRecordID)).ToList();
                    foreach (var i in allSelected)
                    {
                        db.Students.Remove(i);
                    }
                    db.SaveChanges();
                
            }
            return RedirectToAction("index");
        }

        // GET: Students
        public ActionResult Index(string lastName, string firstName, string sID, string citezenship, string agent, string I20exp)
        {           

            var results = from r in db.Students
                         select r;

            if (!String.IsNullOrEmpty(lastName))
            {

                results = results.Where(s => s.LastName.Contains(lastName));
            }

            if (!String.IsNullOrEmpty(firstName))
            {
                
                results = results.Where(s => s.FirstName.Contains(firstName));
            }
            if (!String.IsNullOrEmpty(sID))
            {
                int sIDint = int.Parse(sID);
                results = results.Where(s => s.StudentID == sIDint);
            }
            if (!String.IsNullOrEmpty(citezenship))
            {                
                results = results.Where(s => s.Citezenship.Contains(citezenship));
            }
            if (!String.IsNullOrEmpty(agent))
            {
                results = results.Where(s => s.School_Agent.Contains(agent));
            }
            if (!String.IsNullOrEmpty(I20exp))
            {
                results = results.Where(s => s.I_20_Expiration.ToString().Contains(I20exp));
            }

            return View(results);
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            TempData["studID"] = student.UniqueRecordID;

            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UniqueRecordID,StudentID,FirstName,LastName,LetterGrade,Placement,QuarterOfPlacement,CourseCatalogNumber, SEVIS, I_20_Expiration,gender,DOB,Citezenship,School_Agent,Agent_Email,Transfer_,Telephone,Email,CWU_Email,CWU_Housing,CWU_Address,Home_Address,Emergency_Contact,Emergency_Contact_Relationship,Emergency_Contact_Phone,Emergency_Contact_Email,Conditional_Admission,Status,Mission_Student_ID,Comments,Mission_ID_Expiration")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UniqueRecordID,StudentID,FirstName,LastName,LetterGrade,Placement,QuarterOfPlacement,CourseCatalogNumber, SEVIS, I_20_Expiration,gender,DOB,Citezenship,School_Agent,Agent_Email,Transfer_,Telephone,Email,CWU_Email,CWU_Housing,CWU_Address,Home_Address,Emergency_Contact,Emergency_Contact_Relationship,Emergency_Contact_Phone,Emergency_Contact_Email,Conditional_Admission,Status,Mission_Student_ID,Comments,Mission_ID_Expiration")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
