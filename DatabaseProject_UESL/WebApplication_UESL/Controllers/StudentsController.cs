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
    public class StudentsController : Controller
    {


        public void exportButtonClick(int[] idsToExport)
        {
            var fileDownloadName = "Students.xlsx";

            List<Student> students = new List<Student>();
            if (idsToExport != null && idsToExport.Length != 0)
            {
                students = db.Students.Where(a => idsToExport.Contains(a.UniqueRecordID)).ToList();
            }
            
            

            ExcelExport filemaker = new ExcelExport();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDownloadName);
            Response.BinaryWrite(filemaker.exportStudents(students));
            
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
                    List<studentExportKey> allSelected = new List<studentExportKey>();

                    allSelected = db.studentExportKeys.Where(a => id.Contains(a.studentKey)).ToList();
                                        
                    foreach (var i in allSelected)
                    {
                        db.studentExportKeys.Remove(i);
                    }
                    db.SaveChanges();
                }          
                
            }//end "remove" submit button

            if (submitButton.Equals("exp"))
            {
                //call the exporting class!
                //sending the id array of uniquerecordids of students to export
                exportButtonClick(id);
            }
            return RedirectToAction("index");
        }

        public PartialViewResult studentExport()
        {
            List<studentExportKey> keys = new List<studentExportKey>();
            keys = db.studentExportKeys.ToList();

            List<Student> results = new List<Student>();

            foreach (studentExportKey k in keys)
            {
                results.Add((db.Students.Find(k.studentKey)));
            }

            return PartialView(results);
        }
        

        private DatabaseProject_UESLEntities db = new DatabaseProject_UESLEntities();

        public ActionResult markExport(string[] ids)
        {
         
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public FileContentResult GetImage(int id)
        {
            var student = db.Students.FirstOrDefault(p => p.UniqueRecordID == id);
            if (student != null)
            {
                return File(student.ImageContent, student.ImageMimeType);
            }

            else
            {
                return null;
            }
        }

        public ActionResult DeleteEnrollment(int id, int sRecID)
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
            string toRedirect = "details/" + sRecID;
            return RedirectToAction(toRedirect);
        }

        //partial view that shows currently  enrolled classes for this particular student
        public PartialViewResult enrolledCourses(string ClassName, string classID, string Grade, string Instructor, string quarter, string StudentIDtoPass)
        {
            var results = from r in db.Enrollments
                          select r;

            int sID = (int)TempData["studID"];            

            //this is actually comparing "uniquerecordID" of student because i suck
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

            int studentKey = int.Parse(studentUID);
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
                    //this section makes sure you cant create an identical enrollment twice
                    var checker = from r in db.Enrollments
                                  select r;
                    checker = checker.Where(e => e.CourseID == sint && e.Student.UniqueRecordID == studentKey);
                    if (!(checker.Any()))
                    {
                        // no Match!
                        coursesToAdd.Add(db.Courses.Find(sint));
                    }
                                  
                }
            }

            //gets the student in question
            
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
            if (submitButton.Equals("del")) {
                if (id != null && id.Length > 0)
                {

                    //first i must check and make to sure delete all export keys that refer to the students that i am deleting!
                    //otherwise everything breaks!
                    //do that here now
                    List<studentExportKey> allSelectedKeys = new List<studentExportKey>();
                    allSelectedKeys = db.studentExportKeys.Where(a => id.Contains(a.studentKey)).ToList();

                    foreach(var sk in allSelectedKeys)
                    {
                        db.studentExportKeys.Remove(sk);
                    }

                    List<Student> allSelected = new List<Student>();
                    allSelected = db.Students.Where(a => id.Contains(a.UniqueRecordID)).ToList();

                    foreach (var i in allSelected)
                    {
                        db.Students.Remove(i);
                    }
                    db.SaveChanges();
                }
            }
            //mark for export part
            if (submitButton.Equals("ex"))
            {
                //List<studentExportKey> all = new List<studentExportKey>();
                //all = db.studentExportKeys.ToList(); 
                List<int> all = new List<int>();                
                var temp = db.studentExportKeys.ToList();
                    foreach(studentExportKey k in temp)
                    {
                        all.Add(k.studentKey);
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
                                studentExportKey k = new studentExportKey();

                                k.studentKey = i;
                                db.studentExportKeys.Add(k);
                            }

                        }
                        db.SaveChanges();
                    }
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
        public ActionResult Create([Bind(Include = "UniqueRecordID,StudentID,FirstName,LastName,LetterGrade,Placement,QuarterOfPlacement,CourseCatalogNumber, SEVIS, I_20_Expiration,gender,DOB,Citezenship,School_Agent,Agent_Email,Transfer_,Telephone,Email,CWU_Email,CWU_Housing,CWU_Address,Home_Address,Emergency_Contact,Emergency_Contact_Relationship,Emergency_Contact_Phone,Emergency_Contact_Email,Conditional_Admission,Status,Mission_Student_ID,Comments,Mission_ID_Expiration,ImageContent,ImageMimeType")] Student student, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
               // db.Students.Add(student);
              //  db.SaveChanges();

                if (image != null)
                {
                    student.ImageContent = new byte[image.ContentLength];
                    student.ImageMimeType = image.ContentType;
                    image.InputStream.Read(student.ImageContent, 0, image.ContentLength);
                }
                else
                {
                    // Set the default image:
                    var img = Image.FromFile(Server.MapPath(Url.Content("~/images/no-thumb.png")));

                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    student.ImageContent = new byte[ms.Length];
                    student.ImageMimeType = "image/png";
                    ms.Read(student.ImageContent, 0, (int)ms.Length);
                }

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
        public ActionResult Edit([Bind(Include = "UniqueRecordID,StudentID,FirstName,LastName,LetterGrade,Placement,QuarterOfPlacement,CourseCatalogNumber, SEVIS, I_20_Expiration,gender,DOB,Citezenship,School_Agent,Agent_Email,Transfer_,Telephone,Email,CWU_Email,CWU_Housing,CWU_Address,Home_Address,Emergency_Contact,Emergency_Contact_Relationship,Emergency_Contact_Phone,Emergency_Contact_Email,Conditional_Admission,Status,Mission_Student_ID,Comments,Mission_ID_Expiration")] Student student, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                
          

                if (image != null)
                {
                    student.ImageContent = new byte[image.ContentLength];
                    student.ImageMimeType = image.ContentType;
                    image.InputStream.Read(student.ImageContent, 0, image.ContentLength);
                }
                //get the old image and put it back
                if (image == null)
                {
                    int sid = student.UniqueRecordID;
                    Student temp = db.Students.Find(sid);

                    var local = db.Set<Student>()
                         .Local
                         .FirstOrDefault(f => f.UniqueRecordID == sid);
                    if (local != null)
                    {
                        db.Entry(local).State = EntityState.Detached;

                        student.ImageContent = temp.ImageContent;
                        student.ImageMimeType = temp.ImageMimeType;
                    }
                    
                }

                
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
