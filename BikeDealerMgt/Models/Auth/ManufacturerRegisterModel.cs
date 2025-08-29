using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class ManufacturerRegisterModel:UserRegisterModel
    {

        [Required]
        public string CompanyName { get; set; }

        public string RegistrationNumber { get; set; }
    }
}
