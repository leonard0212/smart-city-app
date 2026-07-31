using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.Departament;
using SmartCity.Web.Models.Departament;
using System.IO;

namespace SmartCity.Web.Config.AutoMapper
{
    public class DepartamentModelConfig : Profile
    {
        public DepartamentModelConfig()
        {
            CreateMap<DepartamentFilterViewModel, DepartamentFilterModel>();
            CreateMap<Departament, DepartamentFilteredViewModel>().ForMember(x => x.TeamsCount, map => map.MapFrom(x => x.Teams.Count));
            CreateMap<Departament, DepartamentInfoViewModel>();

            CreateMap<Team, TeamInfoViewModel>().ForMember(x => x.NotUserMembership, opt => opt.Ignore()).AfterMap((src, dest, context) =>
            {
                dest.UserMembership = new Dictionary<Guid, string>();
                var users = src.TeamMemberships.Select(x => x.User);
                foreach (var user in users)
                {
                    dest.UserMembership.Add(user.Id, user.UserName);
                }
            });


        }

    }
}
