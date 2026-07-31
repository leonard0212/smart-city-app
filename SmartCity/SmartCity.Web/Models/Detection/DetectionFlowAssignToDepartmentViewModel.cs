using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Web.Models.Detection
{
    public class DetectionFlowAssignToDepartmentViewModel
    {



        public IList<SelectListItem> Departaments { get; set; }
        [Display(Name = "Departament")]
        public Guid? DepartamentId { get; set; }


        public Guid? TeamId { get; set; }

        public string Text { get; set; }

    }
}
