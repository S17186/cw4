using Cw4.DTOs.Requests;
using Cw4.DTOs.Responses;
using Cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace Cw4.Service
{
    public class SQLServerDbDAL : IStudentsDAL, IEnrollDAL, IUpgradeDAL
    {

        private const string conString = "Data Source=db-mssql;Initial Catalog=s17186;Integrated Security=True";


        // Upgrades students of given semester and studies
        public IActionResult UpgradeStudents(int semester, string studies)
        {
            int enrollId;
            DateTime startDate;

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand("UpgradeStudentsProcedure", con))
            {

                com.Connection = con;
                con.Open();

                // parametry przekazane w metodzie - parametry INPUT
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("Studies", studies).Direction = ParameterDirection.Input;
                com.Parameters.AddWithValue("Semester", semester).Direction = ParameterDirection.Input;

                // idEnrollment - parametr OUT
                var idEnrollment = com.CreateParameter();
                idEnrollment.Direction = ParameterDirection.Output;
                idEnrollment.ParameterName = "@IdEnrollment";
                idEnrollment.DbType = System.Data.DbType.Int32;
                idEnrollment.Size = 50;
                com.Parameters.Add(idEnrollment);

                // startDate - parametr OUTPUT               
                var StartDate = com.CreateParameter();              
                StartDate.Direction = ParameterDirection.Output;
                StartDate.ParameterName = "@StartDate";
                StartDate.DbType = System.Data.DbType.DateTime;
                StartDate.Size = 50;
                com.Parameters.Add(StartDate);

                // err - parametr bledu
                var err = com.CreateParameter();
                err.Direction = ParameterDirection.Output;
                err.ParameterName = "@Error";
                err.DbType = System.Data.DbType.Int32;
                err.Size = 50;
                com.Parameters.Add(err); 


                // start procedury - liczy, ile wierszy zmienionych
                int rowsAffected = com.ExecuteNonQuery();
                
                // if error occured - return BadRequest
                var error = (int)err.Value;
                if (error>0)
                {
                    string errMsg = "blank error";
                    if (error == 1)
                        errMsg = "Study name not in DB";
                    else if (error == 2)
                        errMsg = "No current enrollment for this studies and semester - nothing to upgrade"; 
                    var badReqMsg = new BadRequestObjectResult(new { message = "401 BadRequest " + errMsg});
                    return badReqMsg;
                }
                else if (rowsAffected == 0)
                {
                    var okMsg = new OkObjectResult(new { message = ("201 OK, but no one to be upgraded - affected " + rowsAffected)});
                    return okMsg;
                }
                else
                {
                    // pobierz wrt parametrow zwrotnych
                    enrollId = (int)idEnrollment.Value;                   
                    startDate = (DateTime)StartDate.Value;

                    // utwórz zwracany obiekt UpgradeStRes oraz wiadomosc 201 OK
                    var newEnroll = new UpgradeStudentsResponse(enrollId, semester + 1, studies, startDate);
                    var okMsg = new OkObjectResult(new { message = ("201 OK, affected "+rowsAffected), newEnroll });

                    return okMsg;
                }
            }
        }


        //Registers new Student
        public DateTime? EnrollStudent(EnrollStudentRequest enrollRequest)
        {
            
            DateTime? enrollDate = null;

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                // Pobierz ID z tabeli Enrollment, gdzie semestr=1               
                com.CommandText = "select top 1 StartDate from Enrollment as e inner join Studies as s on s.IdStudy = e.IdStudy where s.name = @name and e.semester = 1 order by e.startdate desc; ";
                com.Parameters.AddWithValue("name", enrollRequest.StudyName);

                com.Transaction = tran; 
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    enrollDate = (DateTime)dr["StartDate"];
                    dr.Close();
                }
                else
                // Jeżeli nie istnieje, utwórz nowy wpis w Enrollments z dzisiejsza data
                {
                    com.CommandText = "insert into enrollment values ( (select max(idEnrollment) from enrollment)+1, 1, (select idStudy from Studies where name = @name), (SELECT GETDATE()))";
                    com.ExecuteNonQuery();
                }

                // Dodaj studenta
                com.CommandText = "insert into Student values ( @IdStudent, @firstname, @surname, @birthdate, (select idStudy from Studies where name = @studyName))";
                com.Parameters.AddWithValue("IdStudent", enrollRequest.IndexNumber);
                com.Parameters.AddWithValue("firstname", enrollRequest.FirstName);
                com.Parameters.AddWithValue("surname", enrollRequest.LastName);
                com.Parameters.AddWithValue("birthdate", Convert.ToDateTime(enrollRequest.BirthDate));
                com.Parameters.AddWithValue("studyName", enrollRequest.StudyName);
                com.Transaction = tran;
                int done = com.ExecuteNonQuery();

                if (done != 1)
                    tran.Rollback();
                else
                    tran.Commit(); 
            }
            return enrollDate;
        }


        //Checks if StudentID is unique
        public bool StudentIdNonUnique(String id)
        {
            bool unique = false;
            //int exists; 
            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                // sprawdz, czy ID unikalne
                com.CommandText = "select IndexNumber as 'id' from student where IndexNumber=@studentId";
                com.Parameters.AddWithValue("studentId", id);
                com.Connection = con;
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                unique = dr.Read();
                dr.Close();
                //exists = (int) dr["counter"]; 
                //if (exists > 0)
                //    unique = false;
                //else
                //    unique = true; 
            }
            return unique; 
        }


        //Tests if studies of given name exist 
        public bool StudiesExist(String name)
        {
            Boolean exists = false;

            using (var con = new SqlConnection(conString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select idStudy from Studies where name = @name";
                com.Parameters.AddWithValue("name", name);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                exists = dr.Read();
                dr.Close();
            }

            return exists;
        }


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
                    st.Semester = (int) dr["Semester"];

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
                    st.Semester = (int)dr["Semester"];

                    list.Add(st);
                }

            }
            return list;
        }

        
    }
}
