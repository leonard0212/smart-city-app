using AutoMapper;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Domain.ServiceModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Config.AutoMapper
{
    public class UserServiceConfig:Profile
    {
        public UserServiceConfig()
        {
            CreateMap<User, UserFilterResultServiceModel>();
        }


        
    }
}
