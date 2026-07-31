using SmartCity.Domain.Models.Enums;

namespace SmartCity.Web.Models.Billboard
{
    public class BilboardInfoModel
    {
        public Guid Id { get; set; }
        public string BillboardName { get; set; }
        public string BillboardSummary { get; set; }
        public BillboardCategory BillboardCategory { get; set; }
        public BillboardSize BillboardSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
