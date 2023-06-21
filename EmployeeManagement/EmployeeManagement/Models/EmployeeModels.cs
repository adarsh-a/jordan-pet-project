using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeManagement.Models
{
    public class EmployeeModels
    {
        public int EmpID { set; get; }

        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string first_name { set; get; }

        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string last_name { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Required field")]
        public string address { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Required field")]
        [StringLength(8, ErrorMessage = "Phone number should contains 8 digits")]
        [RegularExpression(@"^[5][0-9]{7}$", ErrorMessage = "Phone number should start by 5 and contains 8 digits")]
        public string phone_number { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Required Field")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email address")]
        public string email { get; set; }
        public string status { get; set; }
        public string password { get; set; }
        public string re_password { get; set; }

        [Display(Name = "Role")]
        public int role { get; set; }
    }
}