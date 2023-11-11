using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TwoTablesDataBindingInJqueryDataTable.Models;
using TwoTablesDataBindingInJqueryDataTable.ViewModel;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace TwoTablesDataBindingInJqueryDataTable.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        RamyaEntities db = new RamyaEntities();
        public ActionResult Index()
        {
            List<CourseTable> courseList = db.CourseTables.ToList();
            ViewBag.courseList = new SelectList(courseList, "CourseId", "CourseName");
            return View();
        }
        public ActionResult InlineEditTable()
        {
            List<CourseTable> courseList = db.CourseTables.ToList();
            ViewBag.courses = new SelectList(courseList, "CourseId", "CourseName");
            return View();
        }
        public ActionResult ExpandCollapseTable()
        {
            
            return View();
        }
        public ActionResult GetData()
        {
            using (RamyaEntities db = new RamyaEntities())
            {


                List<StudentTable> studentlist = db.StudentTables.ToList();
                StudentVM studentVM = new StudentVM();
               
                List<StudentVM> studentVMList = (from s in studentlist
                                                 orderby s.StartDate descending
                                                 join c in db.CourseTables on s.CourseId equals c.CourseId

                                                 select new
                                                 StudentVM
                                                 {
                                                     StudentId = s.StudentId,
                                                     StudentName = s.StudentName,
                                                     CourseName = c.CourseName,
                                                     CourseId = c.CourseId,
                                                     StartDate = s.StartDate,
                                                     Grade = s.Grade
                                                 }).ToList();
                


                return Json(new { data = studentVMList }, JsonRequestBehavior.AllowGet);

            }


        }
        public ActionResult GetTable(DateTime? fromDate=null, DateTime? toDate = null,int? Course=null,string Name=null,float ? Grade=null)
        {
            using (RamyaEntities db = new RamyaEntities())
            {

                
                    List<StudentTable> studentlist = db.StudentTables.ToList();
                    StudentVM studentVM = new StudentVM();
                    //List<StudentVM> studentVMList = studentlist.Select(x => new StudentVM
                    //{
                    //    StudentId = x.StudentId,
                    //    StudentName = x.StudentName,
                    //    CourseId=x.CourseId,
                    //    CourseName = x.CourseTable.CourseName,
                    //    StartDate = x.StartDate,
                    //    Grade = x.Grade
                    //}).ToList();


                    //to get all data
                List<StudentVM> studentVMList = (from s in studentlist orderby s.StartDate descending
                                                join c in db.CourseTables on s.CourseId equals c.CourseId

                                                select new
                                                StudentVM
                                                {
                                                    StudentId = s.StudentId,
                                                    StudentName = s.StudentName,
                                                    CourseName = c.CourseName,
                                                    CourseId=c.CourseId,
                                                    StartDate = s.StartDate,
                                                    Grade = s.Grade
                                                }).ToList();
                //date and course
                if (fromDate.HasValue && toDate.HasValue && Course.HasValue)
                {
                    studentVMList = studentVMList.Where(x => x.StartDate >= fromDate && x.StartDate <= toDate && x.CourseId == Course).ToList();
                }
                //only date
                else if (fromDate.HasValue && toDate.HasValue && !Course.HasValue)
                {
                    studentVMList = studentVMList.Where(x => x.StartDate >= fromDate && x.StartDate <= toDate).ToList();
                }
                //only course
                else if (Course.HasValue && !(fromDate.HasValue && toDate.HasValue))
                {
                    studentVMList = studentVMList.Where(x => x.CourseId == Course).ToList();

                }
                //only name or date with name
                if (Name != null && Name !="") {
                    studentVMList = studentVMList.Where(x => x.StudentName == Name).ToList();
                }
                //only grade or grade with date
                if (Grade.HasValue) {
                    studentVMList = studentVMList.Where(x => x.Grade == Grade).ToList();
                }
                

                   return Json(new { data = studentVMList }, JsonRequestBehavior.AllowGet);
                
            }


        }
        //public ActionResult FilterTable(DateTime fromDate, DateTime toDate)
        //{

        //    //List<StudentTable> studentlist = db.StudentTables.ToList();
        //    //StudentVM studentVM = new StudentVM();
        //    List<StudentTable> list1 = db.StudentTables.Where(x => x.StartDate >= fromDate && x.StartDate <= toDate).ToList();
        //    List<StudentVM> filteredList = (from s in list1
        //                                    join c in db.CourseTables on s.CourseId equals c.CourseId

        //                                    select new
        //                                    StudentVM
        //                                    {
        //                                        StudentId = s.StudentId,
        //                                        StudentName = s.StudentName,
        //                                        CourseName = c.CourseName,
        //                                        StartDate = s.StartDate,
        //                                        Grade = s.Grade
        //                                    }).ToList();



        //    return Json(new { data = filteredList }, JsonRequestBehavior.AllowGet);
        //}
        
        public class ListtoDataTableConverter
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcelExport() {
            List<StudentTable> studentlist = db.StudentTables.ToList();
            List<StudentExcelVM> studentExcelVMList = (from s in studentlist
                                             orderby s.StartDate descending
                                             join c in db.CourseTables on s.CourseId equals c.CourseId

                                             select new
                                             StudentExcelVM
                                             {
                                                 StudentName = s.StudentName,
                                                 CourseName = c.CourseName,
                                                 StartDate = s.StartDate,
                                                 Grade = s.Grade
                                             }).ToList();
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable dt = converter.ToDataTable(studentExcelVMList);
            using (var workbook = new XLWorkbook()) {
                //var worksheet = workbook.Worksheets.Add("Students");
                //worksheet.Row(1).Style.Font.Bold = true;
                //var currentRow = 1;
                //worksheet.Cell(currentRow, 1).Value = "Student Name";
                //worksheet.Cell(currentRow, 2).Value = "Course Name";
                //worksheet.Cell(currentRow, 3).Value = "Start Date";
                //worksheet.Cell(currentRow, 4).Value = "Grade";
                //foreach (var stu in studentlist) {
                //    currentRow++;
                //    worksheet.Cell(currentRow, 1).Value = stu.StudentName;
                //    worksheet.Cell(currentRow, 2).Value = stu.CourseTable.CourseName;
                //    worksheet.Cell(currentRow, 3).Value = stu.StartDate;
                //    worksheet.Cell(currentRow, 4).Value = stu.Grade;
                //}
                workbook.Worksheets.Add(dt).ColumnsUsed().AdjustToContents();
                using (var stream=new MemoryStream()) {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
                }
            
            }
           
        }
        public JsonResult IsUserNameAvailable(StudentVM vm)
        {

            if (vm.StudentId == 0)
            {
                return Json(!db.StudentTables.Any(x => x.StudentName == vm.StudentName), JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(!db.StudentTables.Any(x => x.StudentName == vm.StudentName && x.StudentId != vm.StudentId), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.StudentTables.Where(x => x.StudentId == id).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(StudentTable ST)
        {
              if (ST.StudentId == 0)
                {
                    db.StudentTables.Add(ST);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(ST).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
           

        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            StudentTable student = db.StudentTables.Where(x => x.StudentId == id).FirstOrDefault<StudentTable>();
            db.StudentTables.Remove(student);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(string setName, int setCourseId, float setGrade, string setDate,int SendId) {
            StudentTable student = db.StudentTables.Where(x => x.StudentId == SendId).FirstOrDefault<StudentTable>();
            student.StudentName = setName;
            student.CourseId = setCourseId;
            student.Grade = Math.Round(setGrade, 1);
            student.StartDate = Convert.ToDateTime(setDate);
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
           // return View();

        }

    }
}