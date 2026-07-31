using SmartCity.Domain.ServiceModels.TraficSign;
using SmartCity.Domain.ServiceModels.Trash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface ITrafficSignService
    {
        Task CreateTrafficSignDetectionAsync(CreateTrafficSignDetectionRequest request);
    }
}
