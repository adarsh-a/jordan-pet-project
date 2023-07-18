using System;
using System.Collections.Generic;

namespace EmployeeMgtCore
{
    public partial class Tmember
    {
        public int? MemberId { get; set; }
        public int EmpId { get; set; }
        public int? TeamId { get; set; }

        public virtual Employee? Emp { get; set; }
        public virtual Team? Team { get; set; }
    }
}
