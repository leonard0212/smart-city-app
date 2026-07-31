using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum BillboardCategory
    {
        [Description("Comercial")]
        CommercialProduct,
        [Description("Campanie politica")]
        PoliticalCampaign,
        [Description("Jocuri de noroc")]
        Gambling,
    }
}
