using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum TrashVolumType
    {
        [Description("Mic")]
        Small,
        [Description("Mediu")]
        Medium,
        [Description("Mare")]
        Large
    }
}
