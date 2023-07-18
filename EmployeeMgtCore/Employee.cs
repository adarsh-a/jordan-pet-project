using System;
using System.Collections.Generic;

namespace EmployeeMgtCore
{
    public partial class Employee
    {
        public Employee()
        {
            Promotions = new HashSet<Promotion>();
            Teams = new HashSet<Team>();
            Tmembers = new HashSet<Tmember>();
        }

        public int EmpId { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Address { get; set; }
        public string? Phonenum { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Tmember> Tmembers { get; set; }
    }
}
