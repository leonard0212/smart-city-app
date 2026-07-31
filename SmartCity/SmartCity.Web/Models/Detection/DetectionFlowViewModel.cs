namespace SmartCity.Web.Models.Detection
{
    public class DetectionFlowViewModel
    {
        public Guid DetectionId { get; set; }
        public DetectionFlowViewModel()
        {
            Logs = new List<DetectionLogViewModel>();
            AssignToDepartment = new DetectionFlowAssignToDepartmentViewModel();
        }

        public List<DetectionLogViewModel> Logs { get; set; }

        public DetectionFlowAssignToDepartmentViewModel AssignToDepartment { get; set; }
    }
}
