using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.Departament;
using SmartCity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCity.Domain.Models;

namespace SmartCity.Interfaces.Services
{
    public interface IDepartamentService
    {
        Task CreateDepartament(string departametName);
        Task CreateTeam(Guid departamentId, string teamName);
        Task<IPagedList<Departament>> GetIPagedListDepartaments(DepartamentFilterModel model);
        Task<Departament> GetDepartamentById(Guid id);
        Task<(Team, List<TeamMembership>)> GetTeamById(Guid id);

        Task AddUserToTeam(Guid teamId, Guid userId);
        Task RemoveUserFromTeam(Guid teamId, Guid userId);

    }
}
