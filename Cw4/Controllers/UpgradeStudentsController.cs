using System;
using Cw4.DTOs.Requests;
using Cw4.DTOs.Responses;
using Cw4.Service;
using Microsoft.AspNetCore.Mvc;

namespace Cw4.Controllers
{

    [ApiController]
    [Route("api/enrollments/promotion")]

    public class UpgradeStudentsController : ControllerBase
    {
      
        [HttpPost]
        public IActionResult UpgradeStudents(UpgradeStudentsRequest request, [FromServices] IUpgradeDAL dbService)
        {

            //Przelacz na klase SQL
            var response = dbService.UpgradeStudents(request.Semester, request.Studies);
            
            return response;
            
        }
    }
}