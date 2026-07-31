using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services.Common
{
    public interface INomenclatureService
    {
        Task<IList<SelectListItem>> GetRolesList(bool withDefaults = false, Guid? selectedId = null);
        Task<IList<SelectListItem>> GetDepartamentsList(bool withDefaults = false, Guid? selectedId = null);
        Task<IList<SelectListItem>> GetTeamsByDepartmentList(Guid departamentId, bool withDefaults = false, Guid? selectedId = null);
        Task<IList<SelectListItem>> GetEdgeIds(bool withDefaults = false, Guid? selectedId = null);

        Task<IList<SelectListItem>> GetRawDetectionsCategory(bool withDefaults = false, string? selectedId = null);
        Task<IList<SelectListItem>> GetRawDetectionsSubCategory(string mainClass, bool withDefaults = false, string? selectedId = null);
    }
}
