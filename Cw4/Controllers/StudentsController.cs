using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw4.Models;
using Cw4.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw4.Controllers
{
    [Route("api/students")]
    [ApiController]


    public class StudentsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStudents([FromServices] IStudentsDAL dbService)
        {
            var list = dbService.GetStudents();
            return Ok(list);
        }

        
        [HttpGet("{IndexNumber}")]
        public IActionResult GetStudents(string IndexNumber, [FromServices] IStudentsDAL dbService)
        {
            var list = dbService.GetStudents(IndexNumber);
            return Ok(list);
        }

    }
}