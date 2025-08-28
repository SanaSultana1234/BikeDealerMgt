using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class ManufacturerRegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }
        public string Address { get; set; }
    }
}
