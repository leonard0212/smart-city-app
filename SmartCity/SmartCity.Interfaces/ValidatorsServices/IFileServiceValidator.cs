using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ValidatorsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.ValidatorsServices
{
    public interface IFileServiceValidator
    {
        Task ValidateUploadFileAsync(CreateFileIn request, ValidationResult outValidationResult = null);
    }
}
