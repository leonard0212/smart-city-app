using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Exceptions;
using SmartCity.Core.Extensions;
using SmartCity.Core.Utils;
using SmartCity.Domain;
using SmartCity.Domain.Models;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.Departament;
using SmartCity.Domain.ValidatorsServices;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using System.Linq.Expressions;

namespace SmartCity.Core.Services
{
    public class DepartamentService : ServiceBase, IDepartamentService
    {

        private readonly IGenericRepositorySimpleUniqueIdentifier<Departament> _departamentRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<Team> _teamRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<TeamMembership> _teamMembershipRepository;



        public DepartamentService(IApplicationContext<User> applicationContext
            , IGenericRepositorySimpleUniqueIdentifier<Departament> departamentRepository
            , IGenericRepositorySimpleUniqueIdentifier<Team> teamRepository
            , IGenericRepositorySimpleUniqueIdentifier<TeamMembership> teamMembershipRepository
            ) : base(applicationContext)
        {
            _departamentRepository = departamentRepository;
            _teamRepository = teamRepository;
            _teamMembershipRepository = teamMembershipRepository;
        }

        public async Task CreateDepartament(string departametName)
        {
            var existingDepartament = await _departamentRepository.QueryAll().Where(x => x.Name == departametName).FirstOrDefaultAsync();
            if (existingDepartament != null)
                throw new ModelValidationException(new ErrorMessage("Departament already exists"));

            var departament = new Departament
            {
                Name = departametName
            };

            await _departamentRepository.SaveOrUpdateAsync(departament);
            await _departamentRepository.CommitChangesAsync();
        }

        public async Task CreateTeam(Guid departamentId, string teamName)
        {
            var existingDepartament = await _departamentRepository.QueryAll().Where(x => x.Id == departamentId).FirstOrDefaultAsync();
            if (existingDepartament == null)
                throw new ModelValidationException(new ErrorMessage("Departament not exists"));

            var existingTeam = await _teamRepository.QueryAll().Where(x => x.Name == teamName && x.DepartamentId == departamentId).FirstOrDefaultAsync();
            if (existingTeam != null)
                throw new ModelValidationException(new ErrorMessage("Team already exists in this departament"));


            var team = new Team()
            {
                Name = teamName,
                DepartamentId = departamentId
            };
            await _teamRepository.SaveOrUpdateAsync(team);
            await _teamRepository.CommitChangesAsync();
        }



        public async Task<IPagedList<Departament>> GetIPagedListDepartaments(DepartamentFilterModel model)
        {
            var filterResult = FilterDepartaments(model);
            var totalCount = await _departamentRepository.CountAsync(filterResult.where);
            var pagedList = await _departamentRepository
           .GetPagedAsync(model.PageIndex.Value, model.PageSize.Value, filterResult.where, filterResult.order, x => x.Teams)
           .ToPagedListAsync(model.PageIndex.Value, model.PageSize.Value, totalCount, null);
            return pagedList;
        }

        private (Expression<Func<Departament, bool>> where, Func<IQueryable<Departament>, IOrderedQueryable<Departament>> order) FilterDepartaments(DepartamentFilterModel model)
        {
            model.PageIndex = model.PageIndex > 0 ? model.PageIndex : 1;
            model.PageSize = model.PageSize > 0 ? model.PageSize : PagedList.DefaultPageSize;
            var where = QueryPredicateBuilder.True<Departament>();


            if (model.Name != null)
                where = where.And(d => d.Name.Contains(model.Name));

            var order = QueryOrderBuilder.Create<Departament>(query => query.OrderByDescending(x => x.CreatedAt));


            return (where, order);
        }


        public async Task<Departament> GetDepartamentById(Guid id)
        {
            var departament = await _departamentRepository.QueryAll().Where(x => x.Id == id).Include(x => x.Teams).ThenInclude(x => x.TeamMemberships).FirstOrDefaultAsync();
            return departament;
        }


        public async Task<(Team, List<TeamMembership>)> GetTeamById(Guid id)
        {
            var departament = await _teamRepository.QueryAll().Where(x => x.Id == id).Include(x => x.TeamMemberships).FirstOrDefaultAsync();
            var departamentMemberships = _teamMembershipRepository.QueryAll().Include(x => x.User).Where(x => x.TeamId == id).ToList();
            return (departament, departamentMemberships);

        }


        public async Task AddUserToTeam(Guid teamId, Guid userId)
        {

            var existing = await _teamMembershipRepository.QueryAll().FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);
            if (existing == null)
            {
                var newmembership = new TeamMembership() { TeamId = teamId, UserId = userId };
                await _teamMembershipRepository.SaveOrUpdateAsync(newmembership);
                await _teamMembershipRepository.CommitChangesAsync();
            }

        }
        public async Task RemoveUserFromTeam(Guid teamId, Guid userId)
        {

            var existing = await _teamMembershipRepository.QueryAll().FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);
            if (existing != null)
            {
                await _teamMembershipRepository.DeleteAsync(existing);
                await _teamMembershipRepository.CommitChangesAsync();
            }

        }


     




    }
}
