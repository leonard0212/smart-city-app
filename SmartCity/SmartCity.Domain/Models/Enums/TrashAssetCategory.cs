using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum TrashAssetCategory
    {
        [Description("Tomberon necolectat")]
        TrashBagUncollected,
        [Description("Container necolectat")]
        TrashCanUncollected,
        [Description("Container normal")]
        TrashCanOk,
        [Description("Container necolectat")]
        TrashDumpstersUncollected,
        [Description("Container necolectat")]
        TrashUncollected,
    }
}
