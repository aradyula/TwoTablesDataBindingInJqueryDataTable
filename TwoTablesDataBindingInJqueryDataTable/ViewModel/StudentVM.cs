using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwoTablesDataBindingInJqueryDataTable.ViewModel
{
    public class StudentVM
    {
        
        public string CourseName { get; set; }
        public int StudentId { get; set; }
        [Required(ErrorMessage = "*StudentName is Required")]
        [Remote("IsUserNameAvailable","Main",AdditionalFields = "StudentId", ErrorMessage="Student With That Name Already Exists")]
        public string StudentName { get; set; }
        [Required(ErrorMessage = "*Course is Required")]
        [DisplayName("Course")]
        public Nullable<int> CourseId { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [Required(ErrorMessage = "*StartDate is Required")]
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<double> Grade { get; set; }
        public Nullable<int> Fee { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public Nullable<System.DateTime> DOB { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "*Father/Guardian is Required")]
        [DisplayName("Father/Guardian")]
        public string FOGName { get; set; }

    }
}