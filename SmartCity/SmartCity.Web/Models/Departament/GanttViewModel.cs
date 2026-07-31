using SmartCity.Domain.ServiceModels.Departament;

namespace SmartCity.Web.Models.Departament
{
    public class GanttViewModel
    {
        public GanttViewModel()
        {
            GranttData = new List<GranttDataModel>();
            BeginDate = DateTime.Now.AddDays(-3);
            EndDate = DateTime.Now.AddDays(10);
        }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<GranttDataModel> GranttData { get; set; }
    }
}
