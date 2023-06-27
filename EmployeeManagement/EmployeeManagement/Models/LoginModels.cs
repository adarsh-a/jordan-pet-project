using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class LoginModels
    {
        public int id { set; get; }
        public string username { set; get; }

        [DataType(DataType.Password)]
        public string password { set; get; }
    }
}