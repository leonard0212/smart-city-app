using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Enums
{
    public enum DetectionCategory
    {
        [Description("Infrastructura rutiera")]
        RoadInfrastructure,
        [Description("Salubritate")]
        Garbage,
        [Description("Panouri publicitar")]
        Billboards,
        [Description("Semne circulatie")]
        TrafficSign,
    }
}
