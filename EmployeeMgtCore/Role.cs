using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMgtCore
{
    public partial class Role
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
            Promotions = new HashSet<Promotion>();
        }

        [Key]
        public int? RoleId { get; set; }

        [DisplayName("Role name")]
        [Required(ErrorMessage ="Required field")]
        [DataType(DataType.Text)]
        public string? Rolename { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}
