using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ValidatorsServices;
using SmartCity.Interfaces.ValidatorsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.ValidatorsServices
{
    public class FileServiceValidator : ServiceValidatorBase, IFileServiceValidator
    {
        public async Task ValidateUploadFileAsync(CreateFileIn request, ValidationResult outValidationResult = null)
        {
            await ValidationActionWrapperAsync(outValidationResult, async validationResult =>
            {

                if (request.File == null)
                    validationResult.AddError(new ErrorMessage($"File is null.", "File"));
                else if (request.File.Length == 0)
                    validationResult.AddError(new ErrorMessage($"File.Length = 0", "File.Length"));
              


                return validationResult;
            });
        }

    }
}
