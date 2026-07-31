using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.ServiceModels.BillBoard
{
    public class BilboardInfoServiceModel
    {
        public Guid Id { get; set; }
        public string BillboardName { get; set; }
        public string BillboardSummary { get; set; }
        public BillboardCategory BillboardCategory { get; set; }
        public BillboardSize BillboardSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
