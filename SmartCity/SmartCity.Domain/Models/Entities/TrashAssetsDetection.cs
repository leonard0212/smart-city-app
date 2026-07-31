using SmartCity.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Entities
{
    public class TrashAssetsDetection : Detection
    {
        public AssetStatus AssetStatus { get; set; }
        public TrashAssetCategory TrashAssetCategory { get; set; }
        public string TrashDescription { get; set; }
        public string TrashQuantity { get; set; }
        public string TrashCondition { get; set; }
        public TrashVolumType TrashVolumType { get; set; }
    }
}
