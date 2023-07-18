using System.ComponentModel;

namespace EmployeeMgtCore.Models
{
    public class CreateTeamModel
    {
        public int ? Team_ID { get; set; }

        [DisplayName("Team name")]
        public string ? teamname { get; set; }

        [DisplayName("Manager")]
        public string ? mangername { get; set; }
    }
}
