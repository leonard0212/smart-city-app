using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum PotholeType
    {
        [Description("Limitator viteza")]
        SpeedLimiter,
        [Description("Borduri dislocate")]
        DislocatedEdges,
        [Description("Fisura in asfalt")]
        AsphaltCrack,
        [Description("Groapa in asfalt")]
        AsphaltPothole
    }
}
