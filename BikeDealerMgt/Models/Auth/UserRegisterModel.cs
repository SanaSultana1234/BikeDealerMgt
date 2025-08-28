using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth;

public class UserRegisterModel
{
    [Required]
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; }

}
