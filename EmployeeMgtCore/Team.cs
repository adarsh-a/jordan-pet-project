using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMgtCore
{
    public partial class Team
    {
        public Team()
        {
            Tmembers = new HashSet<Tmember>();
        }

        public int ? TeamId { get; set; }

        [DisplayName("Team name")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Text)]
        public string ? Teamname { get; set; }

        [DisplayName("Manager")]
        [Required(ErrorMessage = "Required field")]
        public int ? ManagerId { get; set; }

        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Tmember> Tmembers { get; set; }
    }
}
