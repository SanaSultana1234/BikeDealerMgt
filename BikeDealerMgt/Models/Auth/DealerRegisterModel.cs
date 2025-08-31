using System.ComponentModel.DataAnnotations;

namespace BikeDealerMgtAPI.Models.Auth
{
    public class DealerRegisterModel:UserRegisterModel
    {
        public string StoreName { get; set; }
		[Required]
		public int StorageCapacity { get; set; }
		[Required]
		public int Inventory { get; set; }
	}

}
