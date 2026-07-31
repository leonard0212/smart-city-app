using System.ComponentModel.DataAnnotations;

namespace SmartCity.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The Username field is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
