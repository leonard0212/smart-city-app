namespace SmartCity.Web.Models.Home
{
    public class HomeModel
    {
        public HomeModel()
        {
            DetectionFilterModel = new DetectionFilterModel();
        }

        public int DetectionCount { get; set; }
        public int DetectionAlocatedCount { get; set; }
        public int DetectionUnalocatedCount { get; set; }
        public int DetectionSolvedCount { get; set; }

        public int TrashDetectionCount { get; set; }
        public int TrashDetectionAlocatedCount { get; set; }
        public int TrashDetectionUnalocatedCount { get; set; }
        public int TrashDetectionSolvedCount { get; set; }

        public int RoadInfrastructureDetectionCount { get; set; }
        public int RoadInfrastructureDetectionAlocatedCount { get; set; }
        public int RoadInfrastructureDetectionUnalocatedCount { get; set; }
        public int RoadInfrastructureDetectionSolvedCount { get; set; }

        public int BillboardDetectionCount { get; set; }
        public int BillboardDetectionAlocatedCount { get; set; }
        public int BillboardDetectionUnalocatedCount { get; set; }
        public int BillboardDetectionSolvedCount { get; set; }



        public DetectionFilterModel DetectionFilterModel { get; set; }

    }
}
