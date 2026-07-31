using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class BillboardDetection : Detection
    {
        public string? BillboardName { get; set; }
        public string? BillboardSummary { get; set; }
        public BillboardCategory BillboardCategory { get; set; }
        public BillboardSize BillboardSize { get; set; }
    }
}
