using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class AddMembersModels
    {
        public int emp_ID { set; get; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }

        public string role_name { set; get; }
    }
}