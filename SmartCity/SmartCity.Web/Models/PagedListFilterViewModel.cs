using Microsoft.AspNetCore.Mvc;
using SmartCity.Domain;

namespace SmartCity.Web.Models
{
    public class PagedListFilterViewModel
    {
        public PagedListFilterViewModel()
        {
            PageSize = PagedList.DefaultPageSize;
        }



        public int? PageIndex { get; set; }


        public int? PageSize { get; set; }

        [HiddenInput]
        public int? PersistentCount { get; set; }


        public string Order { get; set; }
    }
}
