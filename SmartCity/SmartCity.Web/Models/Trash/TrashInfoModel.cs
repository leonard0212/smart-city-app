using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.Trash
{
    public class TrashInfoModel
    {
        public Guid Id { get; set; }
        public AssetStatus AssetStatus { get; set; }
        public TrashAssetCategory TrashAssetCategory { get; set; }
        public string TrashDescription { get; set; }
        public string TrashQuantity { get; set; }
        public string TrashCondition { get; set; }
        public TrashVolumType TrashVolumType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
