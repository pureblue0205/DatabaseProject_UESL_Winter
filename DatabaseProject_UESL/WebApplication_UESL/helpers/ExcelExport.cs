using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OfficeOpenXml;
using System.Xml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Web.Mvc;
using WebApplication_UESL.Models;

namespace WebApplication_UESL.helpers
{
    public class ExcelExport
    {
        public Byte[] exportStudents(List<Student> studentsToExport)
        {

            FileInfo newFile = new FileInfo("Students");
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("UESL Students");
                //bio
                worksheet.Cells[1, 1].Value = "Last Name";
                worksheet.Cells[1, 2].Value = "First Name";
                worksheet.Cells[1, 3].Value = "SID";
                worksheet.Cells[1, 4].Value = "SEVIS";
                worksheet.Cells[1, 5].Value = "I-20 Expiration";
                worksheet.Cells[1, 6].Value = "Mission Student ID";
                worksheet.Cells[1, 7].Value = "Mission ID Expiration";

                worksheet.Cells[1, 8].Value = "Gender";
                worksheet.Cells[1, 9].Value = "DOB";
                worksheet.Cells[1, 10].Value = "Citizenship";
                worksheet.Cells[1, 11].Value = "School/Agent";
                worksheet.Cells[1, 12].Value = "Agent Email";
                worksheet.Cells[1, 13].Value = "Transfer?";
                worksheet.Cells[1, 14].Value = "Condtional Admission?";
                worksheet.Cells[1, 15].Value = "status?";
                worksheet.Cells[1, 16].Value = "Placement";
                worksheet.Cells[1, 17].Value = "Placement Quarter";
                //worksheet.Cells[1, 18].Value = "Catalog Number (placement)";

                //contact
                worksheet.Cells[1, 18].Value = "Telephone";
                worksheet.Cells[1, 19].Value = "Email";
                worksheet.Cells[1, 20].Value = "CWU Email";
                worksheet.Cells[1, 21].Value = "CWU Housing?";
                worksheet.Cells[1, 22].Value = "CWU Adress";
                worksheet.Cells[1, 23].Value = "Home Adress";
                worksheet.Cells[1, 24].Value = "Emergency Contact";
                worksheet.Cells[1, 25].Value = "Emergency Contact Relationship";
                worksheet.Cells[1, 26].Value = "Emergency Contact Email";
                worksheet.Cells[1, 27].Value = "Emergency Contact Phone";

                int rowCounter = 2;
                String a, b, c, d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,aa = "";
                foreach (Student stud in studentsToExport)
                {
                    a = "A" + rowCounter.ToString();
                    b = "B" + rowCounter.ToString();
                    c = "C" + rowCounter.ToString();
                    d = "D" + rowCounter.ToString();
                    e = "E" + rowCounter.ToString();
                    f = "F" + rowCounter.ToString();
                    g = "G" + rowCounter.ToString();
                    h = "H" + rowCounter.ToString();
                    i = "I" + rowCounter.ToString();
                    j = "J" + rowCounter.ToString();
                    k = "K" + rowCounter.ToString();
                    l = "L" + rowCounter.ToString();
                    m = "M" + rowCounter.ToString();
                    n = "N" + rowCounter.ToString();
                    o = "O" + rowCounter.ToString();
                    p = "P" + rowCounter.ToString();
                    q = "Q" + rowCounter.ToString();
                    r = "R" + rowCounter.ToString();
                    s = "S" + rowCounter.ToString();
                    t = "T" + rowCounter.ToString();
                    u = "U" + rowCounter.ToString();
                    v = "V" + rowCounter.ToString();
                    w = "W" + rowCounter.ToString();
                    x = "X" + rowCounter.ToString();
                    y = "Y" + rowCounter.ToString();
                    z = "Z" + rowCounter.ToString();
                    aa = "AA" + rowCounter.ToString();
                    
                    
                    worksheet.Cells[a.ToString()].Value = stud.LastName;
                    worksheet.Cells[b.ToString()].Value = stud.FirstName;
                    worksheet.Cells[c.ToString()].Value = stud.StudentID.ToString();
                    worksheet.Cells[d.ToString()].Value = stud.SEVIS;

                    //need to check if i20expiration is null first otherwise you will get error!                    
                    string donly = "";
                    if (stud.I_20_Expiration != null)
                    {
                        var dt = (DateTime)stud.I_20_Expiration;
                        donly = dt.Date.ToShortDateString();
                    }                                       
                    worksheet.Cells[e.ToString()].Value = donly;                                        
                    worksheet.Cells[f.ToString()].Value = stud.Mission_Student_ID;
                                        
                    string donly2 = "";
                    if (stud.Mission_ID_Expiration != null)
                    {
                        var dt2 = (DateTime)stud.Mission_ID_Expiration;
                        donly = dt2.Date.ToShortDateString();
                    }
                    worksheet.Cells[g.ToString()].Value = donly2;
                    worksheet.Cells[h.ToString()].Value = stud.gender;


                    string donly3 = "";
                    if (stud.DOB != null)
                    {
                        var dt3 = (DateTime)stud.DOB;
                        donly3 = dt3.Date.ToShortDateString();
                    }
                    worksheet.Cells[i.ToString()].Value = donly3;

                    worksheet.Cells[j.ToString()].Value = stud.Citezenship;
                    worksheet.Cells[k.ToString()].Value = stud.School_Agent;
                    worksheet.Cells[l.ToString()].Value = stud.Agent_Email;
                    worksheet.Cells[m.ToString()].Value = stud.Transfer_;
                    worksheet.Cells[n.ToString()].Value = stud.Conditional_Admission;
                    worksheet.Cells[o.ToString()].Value = stud.Status;
                    worksheet.Cells[p.ToString()].Value = stud.Placement;
                    worksheet.Cells[q.ToString()].Value = stud.QuarterOfPlacement;
                   // worksheet.Cells[r.ToString()].Value = stud.CourseCatalogNumber;
                    worksheet.Cells[r.ToString()].Value = stud.Telephone;
                    worksheet.Cells[s.ToString()].Value = stud.Email;
                    worksheet.Cells[t.ToString()].Value = stud.CWU_Email;
                    worksheet.Cells[u.ToString()].Value = stud.CWU_Housing;
                    worksheet.Cells[v.ToString()].Value = stud.CWU_Address;
                    worksheet.Cells[w.ToString()].Value = stud.Home_Address;
                    worksheet.Cells[x.ToString()].Value = stud.Emergency_Contact;
                    worksheet.Cells[y.ToString()].Value = stud.Emergency_Contact_Relationship;
                    worksheet.Cells[z.ToString()].Value = stud.Emergency_Contact_Email;
                    worksheet.Cells[aa.ToString()].Value = stud.Emergency_Contact_Phone;
                    rowCounter++;
                }                                    

                //Ok now format the values;
               using (var range = worksheet.Cells[1, 1, 1, 29])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }                

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                // worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.DefaultRowHeight = 25;

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" UESL Students";            
                
                // set some document properties
                package.Workbook.Properties.Title = "Students";
                package.Workbook.Properties.Author = "UESL";                         

                // set some custom property values                
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // return dat package as a byte array
                return package.GetAsByteArray();

            }//using           

        }//export students end

        public Byte[] exportCourses(List<Course> coursesToExport)
        {
            FileInfo newFile = new FileInfo("Courses");
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Courses List");
                //bio
                worksheet.Cells[1, 1].Value = "Catalog #";
                worksheet.Cells[1, 2].Value = "Class Name";
                worksheet.Cells[1, 3].Value = "Class ID";
                worksheet.Cells[1, 4].Value = "Instructor";
                worksheet.Cells[1, 5].Value = "Meet Time";
                worksheet.Cells[1, 6].Value = "Location";
                worksheet.Cells[1, 7].Value = "Quarter";
                            

                int rowCounter = 2;
                String a, b, c, d, e, f, g= "";
                foreach (Course cs in coursesToExport)
                {
                    a = "A" + rowCounter.ToString();
                    b = "B" + rowCounter.ToString();
                    c = "C" + rowCounter.ToString();
                    d = "D" + rowCounter.ToString();
                    e = "E" + rowCounter.ToString();
                    f = "F" + rowCounter.ToString();
                    g = "G" + rowCounter.ToString();

                    worksheet.Cells[a.ToString()].Value = cs.CatalogNumber;
                    worksheet.Cells[b.ToString()].Value = cs.ClassName;
                    worksheet.Cells[c.ToString()].Value = cs.classID;
                    worksheet.Cells[d.ToString()].Value = cs.Instructor;
                    worksheet.Cells[e.ToString()].Value = cs.MeetTime;
                    worksheet.Cells[f.ToString()].Value = cs.Location;
                    worksheet.Cells[g.ToString()].Value = cs.quarter;

                    rowCounter++;
                }

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                // worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.DefaultRowHeight = 25;

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Course List";

                // set some document properties
                package.Workbook.Properties.Title = "Courses";
                package.Workbook.Properties.Author = "UESL";

                // set some custom property values                
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // return dat package as a byte array
                return package.GetAsByteArray();

            }//using 
        }//coursesexport

        public Byte[] exportEnrollments(List<Enrollment> enrollmentsToExport)
        {
            FileInfo newFile = new FileInfo("Enrollments");
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Enrollments List");
                //bio
                worksheet.Cells[1, 1].Value = "Last Name";
                worksheet.Cells[1, 2].Value = "First Name";
                worksheet.Cells[1, 3].Value = "Student ID";
                worksheet.Cells[1, 4].Value = "Class ID";
                worksheet.Cells[1, 5].Value = "Class Name";
                worksheet.Cells[1, 6].Value = "Catlalog #";
                worksheet.Cells[1, 7].Value = "Location";
                worksheet.Cells[1, 8].Value = "Meet Time";
                worksheet.Cells[1, 9].Value = "Quarter";
                worksheet.Cells[1, 10].Value = "Instructor";
                worksheet.Cells[1, 11].Value = "Grade";


                int rowCounter = 2;
                String a, b, c, d, e, f, g ,h,i,j,k= "";
                foreach (Enrollment en in enrollmentsToExport)
                {
                    a = "A" + rowCounter.ToString();
                    b = "B" + rowCounter.ToString();
                    c = "C" + rowCounter.ToString();
                    d = "D" + rowCounter.ToString();
                    e = "E" + rowCounter.ToString();
                    f = "F" + rowCounter.ToString();
                    g = "G" + rowCounter.ToString();
                    h = "H" + rowCounter.ToString();
                    i = "I" + rowCounter.ToString();
                    j = "J" + rowCounter.ToString();
                    k = "K" + rowCounter.ToString();

                    worksheet.Cells[a.ToString()].Value = en.Student.LastName;
                    worksheet.Cells[b.ToString()].Value = en.Student.FirstName;
                    worksheet.Cells[c.ToString()].Value = en.Student.StudentID;
                    worksheet.Cells[d.ToString()].Value = en.Course.classID;
                    worksheet.Cells[e.ToString()].Value = en.Course.ClassName;
                    worksheet.Cells[f.ToString()].Value = en.Course.CatalogNumber;
                    worksheet.Cells[g.ToString()].Value = en.Course.Location;
                    worksheet.Cells[h.ToString()].Value = en.Course.MeetTime;
                    worksheet.Cells[i.ToString()].Value = en.Course.quarter;
                    worksheet.Cells[j.ToString()].Value = en.Course.Instructor;
                    worksheet.Cells[k.ToString()].Value = en.grade;

                    rowCounter++;
                }

                //Ok now format the values;
                using (var range = worksheet.Cells[1, 1, 1, 11])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                    range.Style.Font.Color.SetColor(Color.White);
                }

                //There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                //For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                //you want to use the result of a formula in your program.
                worksheet.Calculate();

                // worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.DefaultRowHeight = 25;

                // lets set the header text 
                worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Course List";

                // set some document properties
                package.Workbook.Properties.Title = "Enrollments";
                package.Workbook.Properties.Author = "UESL";

                // set some custom property values                
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // return dat package as a byte array
                return package.GetAsByteArray();

            }//using 
        }//end enrollments export

    }//class
}