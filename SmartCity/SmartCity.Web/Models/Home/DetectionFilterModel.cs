using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCity.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Web.Models.Home
{
    public class DetectionFilterModel : PagedListFilterViewModel
    {

        public DetectionCategory? Category { get; set; }
        public SeverityType? Severity { get; set; }
        public YesNo? NeedIntervention { get; set; }
        public DetectionResolutionStatus? ResolutionStatus { get; set; }
        public DateTime? ReportingDateStart { get; set; }
        public DateTime? ReportingDateEnd { get; set; }

        public IList<SelectListItem> Departaments { get; set; }
        [Display(Name = "Departament")]
        public Guid? DepartamentId { get; set; }
    }
}
