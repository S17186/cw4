using Cw4.DTOs.Requests;
using Cw4.DTOs.Responses;
using Cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Service
{
    public interface IEnrollDAL
    {
        public bool StudiesExist(String name);
        public bool StudentIdNonUnique(String Id); 
        public DateTime? EnrollStudent(EnrollStudentRequest enrollRequest);
    }
}
