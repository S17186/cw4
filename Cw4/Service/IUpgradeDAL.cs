using Cw4.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Service
{
    public interface IUpgradeDAL
    {
        public IActionResult UpgradeStudents(int semester, string studies);
    }
}
