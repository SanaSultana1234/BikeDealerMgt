using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class DealerRegisterModel:UserRegisterModel
    {

        [Required]
        public string StoreName { get; set; }

        public string GSTNumber { get; set; }
    }

}
