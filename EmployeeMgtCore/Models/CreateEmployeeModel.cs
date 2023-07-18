using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMgtCore.Models
{
    public class CreateEmployeeModel
    {
        public int? emp_ID { set; get; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string? fname { set; get; }

        [DisplayName("Last name")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Should contains only letters")]
        public string? lname { set; get; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Required field")]
        public string? address { set; get; }

        [DisplayName("Phone number")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(8, ErrorMessage = "Phone number should contains 8 digits")]
        [RegularExpression(@"^[5][0-9]{7}$", ErrorMessage = "Start by 5 and contains 8 digits")]
        public string? phonenum { set; get; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email address")]
        public string? email { set; get; }

        [DisplayName("Role")]
        [Required(ErrorMessage = "Required field")]
        public int? role_ID { set; get; }
    }
}
