using System;
using System.Collections.Generic;

namespace EmployeeMgtCore.DataDB
{
    public partial class Promotion
    {
        public int PromotionId { get; set; }
        public int? EmpId { get; set; }
        public int? Oldrole { get; set; }
        public int? Newrole { get; set; }
        public DateTime? Datecreated { get; set; }

        public virtual Employee? Emp { get; set; }
        public virtual Role? NewroleNavigation { get; set; }
    }
}
