using Cw4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.Service
{
    public interface IStudentsDAL
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Student> GetStudents(string indexNumber);
    }
}
