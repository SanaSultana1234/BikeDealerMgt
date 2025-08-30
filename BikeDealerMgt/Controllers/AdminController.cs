using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")] // Only Admin can access these endpoints
	public class AdminController : ControllerBase
	{
		private readonly UserManager<AuthUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(UserManager<AuthUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}


		///Approve Manufacturer
		[HttpPost("approve-manufacturer/{manufacturerId}")]
		public async Task<IActionResult> ApproveManufacturer(string manufacturerId)
		{
			var manufacturer = await _userManager.FindByIdAsync(manufacturerId);
			if (manufacturer == null)
				return NotFound(new { Status = "Error", Message = "Manufacturer not found" });


			// Ensure Manufacturer role exists
			if (!await _roleManager.RoleExistsAsync(UserRoles.Manufacturer))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manufacturer));

			manufacturer.IsManufacturerVerified = true;
			var result = await _userManager.UpdateAsync(manufacturer);
			await _userManager.AddToRoleAsync(manufacturer, UserRoles.Manufacturer);

			if (!result.Succeeded)
				return BadRequest(new { Status = "Error", Message = "Failed to approve manufacturer", Errors = result.Errors });

			return Ok(new { Status = "Success", Message = $"Manufacturer {manufacturer.UserName} approved successfully" });
		}

		///Assign Admin Role to a User
		[HttpPost("assign-admin-role/{userId}")]
		public async Task<IActionResult> AssignAdminRole(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return NotFound(new { Status = "Error", Message = "User not found" });

			// Ensure Admin role exists
			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

			var result = await _userManager.AddToRoleAsync(user, UserRoles.Admin);
			if (!result.Succeeded)
				return BadRequest(new { Status = "Error", Message = "Failed to assign Admin role", Errors = result.Errors });

			return Ok(new { Status = "Success", Message = $"User {user.UserName} has been granted Admin role" });
		}

		///List all Users with their Roles
		[HttpGet("list-users")]
		public async Task<IActionResult> ListUsers()
		{
			var users = _userManager.Users.ToList();
			var userList = new List<object>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userList.Add(new
				{
					user.Id,
					user.UserName,
					user.Email,
					user.Address,
					user.StoreName,
					user.CompanyName,
					Roles = roles,
					user.IsDealerVerified,
					user.IsManufacturerVerified
				});
			}

			return Ok(userList);
		}
	}
}




//Not to complicate things right now
///Approve Dealer
//[HttpPost("approve-dealer/{dealerId}")]
//public async Task<IActionResult> ApproveDealer(string dealerId)
//{
//	var dealer = await _userManager.FindByIdAsync(dealerId);
//	if (dealer == null)
//		return NotFound(new { Status = "Error", Message = "Dealer not found" });

//	// Ensure Dealer role exists
//	if (!await _roleManager.RoleExistsAsync(UserRoles.Dealer))
//		await _roleManager.CreateAsync(new IdentityRole(UserRoles.Dealer));

//	dealer.IsDealerVerified = true;
//	var result = await _userManager.UpdateAsync(dealer);
//	await _userManager.AddToRoleAsync(dealer, UserRoles.Dealer);
//	if (!result.Succeeded)
//		return BadRequest(new { Status = "Error", Message = "Failed to approve dealer", Errors = result.Errors });

//	return Ok(new { Status = "Success", Message = $"Dealer {dealer.UserName} approved successfully" });
//}