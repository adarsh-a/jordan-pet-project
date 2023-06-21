using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Models
{
    public class EditEmployeeModels
    {
        public int emp_ID { set; get; }

        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string first_name { get; set; }

        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string last_name { set; get; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Required field")]
        public string address { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Required field")]
        [StringLength(8, ErrorMessage = "Phone number should contains 8 digits")]
        [RegularExpression(@"^[5][0-9]{7}$", ErrorMessage = "Phone number should start by 5 and contains 8 digits")]
        public string phone_number { set; get; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Required Field")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email address")]
        public string email { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Required field")]
        public string status { set; get; }
        public string password { get; set; }

        [Display(Name = "Role")]
        //[Required(ErrorMessage = "Required field")]
        public int role_ID { set; get; }
    }
}