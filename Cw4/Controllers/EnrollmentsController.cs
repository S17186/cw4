using System;
using Cw4.DTOs.Requests;
using Cw4.DTOs.Responses;
using Cw4.Service;
using Microsoft.AspNetCore.Mvc;

namespace Cw4.Controllers
{
    
    [ApiController]
    [Route("api/enrollments")]

    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request, [FromServices] IEnrollDAL dbService)
        {

            //======ZADANIE 1===================
  
            //Pola wymagane z zapytania POST w tworzeniu EnrollStReq...-
            //błąd 400 zwracany automatycznie z adnotacji w klasie EnrollStudentReq...

            // Sprawdzenie, czy studia istnieją
            if (!dbService.StudiesExist(request.StudyName))           
                return BadRequest("Studies do not exist in DB");

            // Sprawdz, czy studentId unikalne
            if(dbService.StudentIdNonUnique(request.IndexNumber))
                return BadRequest("Student Id not unique");

            // Dodaj studenta 
            DateTime? enrollDate = dbService.EnrollStudent(request);
            if (enrollDate.Equals(null))
                return Unauthorized("Error when registering in DB - failed to enroll");
            // Nie wiem, jaki error tu dac. W sumie to nie jest Badrequest, bo to bylby blad polaczenia z baza

            //Wyslij obiekt EnrollmentResponse
            EnrollStudentResponse enroll = new EnrollStudentResponse(request.IndexNumber, request.LastName, 1, enrollDate.GetValueOrDefault()); 
            
            var result = new OkObjectResult(new { message = "201 OK", enroll});
            return result;     
            
        }
    }
}