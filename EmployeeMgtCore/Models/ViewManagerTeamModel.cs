using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class ViewManagerTeamModel
    {
        public int? empID { get; set; }

        [DisplayName("Employee name")]
        public string? empname { get; set; }

        [DisplayName("Email")]
        public string ? email { get; set; }

        [DisplayName("Phone Number")]
        public string ? phone { get; set; }

        [DisplayName("Role")]
        public string? rolename { get; set;}
    }
}
