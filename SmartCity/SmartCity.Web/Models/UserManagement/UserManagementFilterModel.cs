using Microsoft.AspNetCore.Mvc.Rendering;
using SmartCity.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartCity.Web.Models.UserManagement
{
    public class UserManagementFilterModel : PagedListFilterViewModel
    {

        public string Username { get; set; }
   

    }
}
