using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username or Email is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
