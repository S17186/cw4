using System;


namespace Cw4.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public string IndexNumber { get; set; }
        public string LastName { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }

        public EnrollStudentResponse(String ID, String lastname, int sem, DateTime startDate)
        {
            IndexNumber = ID;
            LastName = lastname;
            Semester = sem;
            StartDate = startDate; 
        }
    }
}
