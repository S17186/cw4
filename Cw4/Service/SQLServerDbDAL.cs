using Cw4.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Service
{
    public class SQLServerDbDAL : IStudentsDAL
    {
        private const string conString = "Data Source=db-mssql;Initial Catalog=s17186;Integrated Security=True";
        
        //Returns a list of students enrolled at the same semester as student with given indexNumber
        public IEnumerable<Student> GetStudents(string indexNumber)
        {
            var list = new List<Student>();

            using (var con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.Parameters.AddWithValue("index", value: indexNumber);


                com.CommandText = "select distinct s.IndexNumber, s.FirstName, s.LastName, st.Name, e.Semester from student as s join Enrollment as e on s.IdEnrollment = e.IdEnrollment join Studies as st on st.IdStudy = e.IdStudy where e.Semester=(select semester from Enrollment join Student on Enrollment.IdEnrollment = Student.IdEnrollment where Student.IndexNumber=@index ) order  by s.IndexNumber";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {

                    Student st = new Student();

                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = dr["Semester"].ToString();

                    list.Add(st);
                }
            }
            return list;
        }

        //Returns a list of all students
        public IEnumerable<Student> GetStudents()
        {
            var list = new List<Student>();

            using (var con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select distinct s.IndexNumber, s.FirstName, s.LastName, st.Name, e.Semester from student as s join Enrollment as e on s.IdEnrollment = e.IdEnrollment join Studies as st on st.IdStudy = e.IdStudy order by s.IndexNumber";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    Student st = new Student();

                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = dr["Semester"].ToString();

                    list.Add(st);
                }

            }
            return list;
        }
    }
}
