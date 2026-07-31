using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCity.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Web.Models.Admin
{
    public class RawDetectionsFilterModel : PagedListFilterViewModel
    {
        public IList<SelectListItem> EdgeIds { get; set; }

        [Display(Name = "Edge")]
        public Guid? EdgeId { get; set; }


        public IList<SelectListItem> Category { get; set; }

        [Display(Name = "Category")]
        public string? CategoryId { get; set; }


        public IList<SelectListItem> SubCategory { get; set; }

        [Display(Name = "SubCategory")]
        public string? SubCategoryId { get; set; }


        public RawDetectionProcessStatus? Status { get; set; }
        

    }
}
