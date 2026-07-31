using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum AssetStatus
    {
        [Description("Normal")]
        Normal,
        [Description("Alertă")]
        Alert
    }
}
