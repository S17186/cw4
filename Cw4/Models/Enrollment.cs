using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Models
{
    public class Enrollment
    {
        public String StudentsID { get; set; }
        public String StudyName { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
    }
}
