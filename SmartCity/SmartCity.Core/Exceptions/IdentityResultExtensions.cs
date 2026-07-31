using Microsoft.AspNetCore.Identity;

namespace SmartCity.Core.Exceptions
{
    public static class IdentityResultExtensions
    {
        public static IEnumerable<string> GetErrorMessages(this IdentityResult result)
        {
            var errorMessages = result.Errors.Select(x => x.Description);
            return errorMessages;
        }
    }
}
