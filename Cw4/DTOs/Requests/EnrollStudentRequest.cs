using Cw4.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        [RegularExpression("^\\d+$")]
        public string IndexNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }
        [Required]
       // [RegularExpression("^[\\d\\d.\\d\\d.\\d\\d\\d\\d]$")]
        public string BirthDate { get; set; }
        [Required]
        public string StudyName { get; set; }

    }
}
