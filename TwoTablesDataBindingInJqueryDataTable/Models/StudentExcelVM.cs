using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwoTablesDataBindingInJqueryDataTable.Models
{
    public class StudentExcelVM
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<double> Grade { get; set; }
    }
}