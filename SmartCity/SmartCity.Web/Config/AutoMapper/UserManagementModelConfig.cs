using AutoMapper;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.User;
using SmartCity.Web.Models.Billboard;
using SmartCity.Web.Models.UserManagement;

namespace SmartCity.Web.Config.AutoMapper
{
    public class UserManagementModelConfig : Profile
    {
        public UserManagementModelConfig()
        {
            CreateMap<UserManagementFilterModel, UserFilterServiceModel>();
            CreateMap<UserFilterResultServiceModel, UserModel>();
            CreateMap<CreateUserModel, CreateUserIn>();

            
        }


    }
}
