using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class DeleteTeamModel
    {
        public int ? teamID { get; set; }

        [DisplayName("Team name")]
        public string ? teamname { get; set; }

        [DisplayName("Manager")]
        public string ? managername { get; set; }

    }
}
