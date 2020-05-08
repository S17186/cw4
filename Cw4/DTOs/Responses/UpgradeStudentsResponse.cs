using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.DTOs.Responses
{
    public class UpgradeStudentsResponse
    {
        public int EnrollmentId { get; set; }
        public int Semester { get; set; }
        public string Studies { get; set; }
        public DateTime StartDate { get; set; }


        public UpgradeStudentsResponse(int e_ID, int sem, string studies, DateTime startDate)
        {
            EnrollmentId = e_ID;
            Semester = sem;
            Studies = studies;
            StartDate = startDate;
        }
    }
}
