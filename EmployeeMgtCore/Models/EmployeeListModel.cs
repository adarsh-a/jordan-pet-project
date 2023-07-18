using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class EmployeeListModel
    {
        public int emp_ID { set; get; }

        [DisplayName("First name")]
        public string? fname { set; get; }

        [DisplayName("Last name")]
        public string? lname { set; get;}

        [DisplayName("Address")]
        public string? address { set; get; }

        [DisplayName("Phone number")]
        public string? phonenum { set; get; }

        [DisplayName("Email")]
        public string? email { set; get; }

        [DisplayName("Role")]
        public string? rolename { set; get; }
    }
}
