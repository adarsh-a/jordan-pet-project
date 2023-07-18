using System;
using System.Collections.Generic;

namespace EmployeeMgtCore.DataDB
{
    public partial class Team
    {
        public Team()
        {
            Tmembers = new HashSet<Tmember>();
        }

        public int TeamId { get; set; }
        public string? Teamname { get; set; }
        public int? ManagerId { get; set; }

        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Tmember> Tmembers { get; set; }
    }
}
