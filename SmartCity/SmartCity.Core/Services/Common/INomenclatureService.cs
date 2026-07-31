using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartCity.Core.Extensions;
using SmartCity.Core.Utils;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services.Common;

namespace SmartCity.Core.Services.Common
{
    public class NomenclatureService : ServiceBase, INomenclatureService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IGenericRepositorySimpleUniqueIdentifier<Departament> _departamentRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<Team> _teamRepository;
        private readonly string _connectionString;

        public NomenclatureService(IApplicationContext<User> applicationContext
            , RoleManager<Role> roleManager
            , IGenericRepositorySimpleUniqueIdentifier<Departament> departamentRepository
            , IGenericRepositorySimpleUniqueIdentifier<Team> teamRepository
            , IConfiguration _configuration
            ) : base(applicationContext)
        {
            _roleManager = roleManager;
            _departamentRepository = departamentRepository;
            _connectionString = _configuration["database:connection"];
            _teamRepository = teamRepository;
        }

        public async Task<IList<SelectListItem>> GetRolesList(bool withDefaults = false, Guid? selectedId = null)
        {

            var where = QueryPredicateBuilder.True<Role>();


            var roles = await _roleManager.Roles.Where(where).ToListAsync();

            if (withDefaults)
            {
                var withDefaultsresult = roles.ToSelectListWithDefault(x => x.Name.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
                return withDefaultsresult;
            }

            var result = roles.ToSelectListWithoutDefault(x => x.Name.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
            return result;
        }



        public async Task<IList<SelectListItem>> GetDepartamentsList(bool withDefaults = false, Guid? selectedId = null)
        {

            var where = QueryPredicateBuilder.True<Departament>();

            var departaments = await _departamentRepository.GetManyAsync(where);
            if (withDefaults)
            {
                var withDefaultsresult = departaments.ToSelectListWithDefault(x => x.Id.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
                return withDefaultsresult;
            }

            var result = departaments.ToSelectListWithoutDefault(x => x.Id.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
            return result;
        }



        public async Task<IList<SelectListItem>> GetTeamsByDepartmentList(Guid departamentId, bool withDefaults = false, Guid? selectedId = null)
        {
            var where = QueryPredicateBuilder.True<Team>();
            where = where.And(x => x.DepartamentId == departamentId);

            var teams = await _teamRepository.GetManyAsync(where);



            if (withDefaults)
            {
                var withDefaultsresult = teams.ToSelectListWithDefault(x => x.Id.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
                return withDefaultsresult;
            }

            var result = teams.ToSelectListWithoutDefault(x => x.Id.ToString(), x => x.Name, x => x.Id == selectedId, x => x.Name);
            return result;
        }




        public async Task<IList<SelectListItem>> GetEdgeIds(bool withDefaults = false, Guid? selectedId = null)
        {

            var sql = $"select DeviceUid, DeviceName from JetsonDevices";
            IEnumerable<dynamic> edgeIds;
            using (var connection = new SqlConnection(_connectionString))
            {
                edgeIds = await connection.QueryAsync<dynamic>(sql);


                if (withDefaults)
                {
                    var withDefaultsresult = edgeIds.ToSelectListWithDefault(x => x.DeviceUid.ToString(), x => x.DeviceName.ToString(), x => x.DeviceUid == selectedId, x => x.DeviceName);
                    return withDefaultsresult;
                }

                var result = edgeIds.ToSelectListWithoutDefault(x => x.DeviceUid.ToString(), x => x.DeviceName.ToString(), x => x.DeviceUid == selectedId, x => x.DeviceName);
                return result;
            }

        }

        public async Task<IList<SelectListItem>> GetRawDetectionsCategory(bool withDefaults = false, string? selectedId = null)
        {

            var sql = $"select distinct MainClass from RawDetection where MainClass is not null";
            IEnumerable<string> categories;
            using (var connection = new SqlConnection(_connectionString))
            {
                categories = await connection.QueryAsync<string>(sql);


                if (withDefaults)
                {
                    var withDefaultsresult = categories.ToSelectListWithDefault(x => x.ToString(), x => x.ToString(), x => x == selectedId, x => x);
                    return withDefaultsresult;
                }

                var result = categories.ToSelectListWithoutDefault(x => x.ToString(), x => x.ToString(), x => x == selectedId, x => x);
                return result;
            }

        }

        public async Task<IList<SelectListItem>> GetRawDetectionsSubCategory(string mainClass, bool withDefaults = false, string? selectedId = null)
        {

            var sql = $"select distinct SubClass from RawDetection where MainClass = '{mainClass}' and SubClass is not null";
            IEnumerable<string> subcategories;
            using (var connection = new SqlConnection(_connectionString))
            {
                subcategories = await connection.QueryAsync<string>(sql);


                if (withDefaults)
                {
                    var withDefaultsresult = subcategories.ToSelectListWithDefault(x => x.ToString(), x => x.ToString(), x => x == selectedId, x => x);
                    return withDefaultsresult;
                }

                var result = subcategories.ToSelectListWithoutDefault(x => x.ToString(), x => x.ToString(), x => x == selectedId, x => x);
                return result;
            }

        }



    }
}
