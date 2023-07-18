using System;
using System.Collections.Generic;

namespace EmployeeMgtCore.DataDB
{
    public partial class Role
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
            Promotions = new HashSet<Promotion>();
        }

        public int RoleId { get; set; }
        public string? Rolename { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
