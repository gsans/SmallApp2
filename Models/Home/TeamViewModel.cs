using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using SmallApp2.Data;

namespace SmallApp2.Models.Home
{
    public class TeamViewModel: TeamMember
    {
        [DisplayName("Team")]
        public string TeamName { get; set; }

    }
}