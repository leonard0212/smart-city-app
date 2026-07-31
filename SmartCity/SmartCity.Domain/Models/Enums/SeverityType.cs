using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum SeverityType
    {
        [Description("Medie")]
        Medium,
        [Description("Ridicata")]
        High,
        [Description("Scazuta")]
        Low,
        [Description("Critica")]
        Critical,
        [Description("Remediata")]
        Resolved,
        [Description("NA")]
        None
    }
}
