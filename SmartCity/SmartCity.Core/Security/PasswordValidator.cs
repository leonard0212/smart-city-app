using SmartCity.Database;
using SmartCity.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SmartCity.Core.Security
{
    public class PasswordValidator : IPasswordValidator<User>
    {
        private readonly DatabaseContext _applicationDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfigurationSection _passConfig;

        public PasswordValidator(
            DatabaseContext applicationDbContext,
            IPasswordHasher<User> passwordHasher,
            IConfigurationRoot configuration)
        {
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _passConfig = configuration.GetSection("security:password");
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User applicationUser, string password)
        {
            var user = applicationUser ?? await _applicationDbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == applicationUser.Id);

            // length and characters
            if (password.Length < int.Parse(_passConfig["requiredLength"]))
                return IdentityResult.Failed(new IdentityError { Description = $"Parola trebuie sa contina minim {_passConfig["requiredLength"]} caractere." });

            if (_passConfig["requireNonAlpha"] == "true" && password.All(char.IsLetter))
                return IdentityResult.Failed(new IdentityError { Description = "Parola trebuie sa contina minim o cifră sau un caracter special." });

            if (_passConfig["requireUppercase"] == "true" && password.Where(char.IsLetter).All(char.IsLower))
                return IdentityResult.Failed(new IdentityError { Description = "Parola trebuie sa contina minim o litera mare." });

            if (_passConfig["requireLowercase"] == "true" && !password.Where(char.IsLetter).Any(char.IsLower))
                return IdentityResult.Failed(new IdentityError { Description = "Parola trebuie sa contina minim o litera mica." });

            // name
            if (_passConfig["checkNameInPassword"] == "true" && password.ToLower().Contains(user.LastName))
                return IdentityResult.Failed(new IdentityError { Description = "Parola nu trebuie sa contina numele utilizatorului." });

            if (_passConfig["checkNameInPassword"] == "true" && password.ToLower().Contains(user.FirstName))
                return IdentityResult.Failed(new IdentityError { Description = "Parola nu trebuie sa contina prenumele utilizatorului." });

            // password history
            var historyPasswordsToVerify = int.Parse(_passConfig["historyPasswordsToVerify"]);
            var lastPasswords = await _applicationDbContext.Set<PasswordHistory>()
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Take(historyPasswordsToVerify)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            if (lastPasswords.Any(x => _passwordHasher.VerifyHashedPassword(applicationUser, x.PasswordHash, password) == PasswordVerificationResult.Success))
                return IdentityResult.Failed(new IdentityError { Description = $"Parola nu trebuie sa fie identica cu niciuna din ultimele {historyPasswordsToVerify} parole utilizate." });

            return IdentityResult.Success;
        }
    }
}
