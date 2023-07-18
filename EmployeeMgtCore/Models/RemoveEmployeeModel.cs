using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class RemoveEmployeeModel
    {
        public int empID { get; set;}

        [DisplayName("Employee")]
        public string ? empname { get; set;}

        [DisplayName("Email address")]
        public string? email { get; set;}

        [DisplayName("Role")]
        public string? rolename { get; set;}

        [DisplayName("Team")]
        public string ? teamname { get; set;}
    }
}
