using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class DealerRegisterModel
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
        public string BusinessName { get; set; }

        public string GSTNumber { get; set; }
        public string Address { get; set; }
    }

}
