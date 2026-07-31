
using SmartCity.Core.Exceptions;
using SmartCity.Domain.ValidatorsServices;

namespace SmartCity.Core.ValidatorsServices
{
    public class ServiceValidatorBase
    {
        protected async Task ValidationActionWrapperAsync(ValidationResult outValidationResult, Func<ValidationResult, Task<ValidationResult>> validationFuncAsync)
        {
            var shouldThrowException = outValidationResult == null;
            outValidationResult = outValidationResult ?? new ValidationResult();

            await validationFuncAsync(outValidationResult);

            if (!outValidationResult.Valid && shouldThrowException)
                throw new ModelValidationException(outValidationResult.Errors);
        }
    }
}
