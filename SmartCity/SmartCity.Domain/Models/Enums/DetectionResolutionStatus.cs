using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum DetectionResolutionStatus
    {
        [Description("Raportată")]
        Reported,
        [Description("Rezolvată")]
        Solved,
        [Description("In curs de rezolvare")]
        InProgress,
        [Description("Refuzată")]
        Rejected
    }
}
