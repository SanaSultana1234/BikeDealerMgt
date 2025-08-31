using Microsoft.AspNetCore.Identity;

namespace BikeDealerMgtAPI.Models
{
	public class AuthUser : IdentityUser
	{

		// Common Address Field
		public string? Address { get; set; }

		// Dealer-specific fields
		public string? StoreName { get; set; }
		public int? StorageCapacity { get; set; } = 0;
		public int? Inventory { get; set; } = 0;
		public bool IsDealerVerified { get; set; } = false;

		// Manufacturer-specific fields
		public string? CompanyName { get; set; }
		public string? RegistrationNumber { get; set; }
		public bool IsManufacturerVerified { get; set; } = false;
	}
}
