using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class EditProfileModels
    {
        public int emp_ID { set; get; }
        public int role_ID { set; get; }

        [Display(Name = "First Name")]
        public string first_name { set; get; }

        [Display(Name = "Last Name")]
        public string last_name { set; get; }

        [Display(Name = "Address")]
        public string address { set; get; }

        [Display(Name = "Phone Number")]
        public string phone_number { set; get; }

        [Display(Name = "Email")]
        public string email { set; get; }
        public string status { set; get; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password should be 8 charaters long, consisting of at least 1 uppercase, 1 lowercase, 1 numeric character and 1 special character")]
        public string password { set; get; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Confirm password does not match password")]
        public string re_password { set; get; }
    }
}