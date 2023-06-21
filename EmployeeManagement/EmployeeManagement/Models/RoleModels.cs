using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class RoleModels
    {
        public int role_ID { set; get; }

        [Display(Name = "Role name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required field")]
        public string role_name { set; get; }
    }
}