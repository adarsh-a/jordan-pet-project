using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class ManageMembersModel
    {
        public int memberId { get; set; }

        [DisplayName("Employee")]
        public string? membername { get; set; }

        [DisplayName("Email address")]
        public string? email { get; set; }

        [DisplayName("Role")]
        public string? rolename { get; set;}
    }
}
