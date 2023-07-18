using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMgtCore.Models
{
    public class LoginModel
    {
        [DisplayName("Email Address")]
        public string? email { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string? password { get; set; }
    }
}
