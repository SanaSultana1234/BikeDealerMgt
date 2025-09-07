using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Only Admin can access these endpoints
    [EnableCors]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BikeDealerMgmtDbContext _context;

        public AdminController(UserManager<AuthUser> userManager, RoleManager<IdentityRole> roleManager, BikeDealerMgmtDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        //Approve Dealer
        [HttpPost("approve-dealer/{dealerId}")]
        public async Task<IActionResult> ApproveDealer(string dealerId)
        {
            var dealer = await _userManager.FindByIdAsync(dealerId);
            if (dealer == null)
                return NotFound(new { Status = "Error", Message = "Dealer not found" });


            // Ensure Dealer role exists
            if (!await _roleManager.RoleExistsAsync(UserRoles.Dealer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Dealer));

            await _userManager.AddToRoleAsync(dealer, UserRoles.Dealer);

            dealer.IsDealerVerified = true;
            var result = await _userManager.UpdateAsync(dealer);
            await _userManager.AddToRoleAsync(dealer, UserRoles.Dealer);

            // Check if dealer already exists in Dealers table
            var existingDealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == dealer.Id);

            if (existingDealer == null)
            {
                // Create new dealer entry
                var dealer2 = new Dealer
                {
                    UserId = dealer.Id,
                    DealerName = dealer.UserName!,
                    Address = dealer.Address ?? string.Empty,
                    City = null,
                    State = null,
                    ZipCode = null,
                    StorageCapacity = dealer.StorageCapacity ?? 0,
                    Inventory = dealer.Inventory ?? 0
                };

                _context.Dealers.Add(dealer2);
            }
            else
            {
                // Maybe update details instead of inserting again
                existingDealer.DealerName = dealer.UserName!;
                existingDealer.Address = dealer.Address ?? string.Empty;
                existingDealer.StorageCapacity = dealer.StorageCapacity ?? 0;
                existingDealer.Inventory = dealer.Inventory ?? 0;

                _context.Dealers.Update(existingDealer);
            }

            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();

            if (!result.Succeeded)
                return BadRequest(new { Status = "Error", Message = "Failed to approve dealer", Errors = result.Errors });

            return Ok(new { Status = "Success", Message = $"Dealer {dealer.UserName} approved successfully!" });
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

        //delete user. If user is dealer then delete from dealers table too
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            // 1. Find user by Id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Status = "Error", Message = "User not found" });

            // 2. Delete dealer record if exists
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (dealer != null)
            {
                _context.Dealers.Remove(dealer);
                await _context.SaveChangesAsync();
            }

            // 3. Remove user roles
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User and related dealer deleted successfully" });
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

        /// <summary>
        /// Unassign a role (Admin or Dealer) from a user
        /// </summary>

        [HttpPost("unassign-role/{userId}/{roleName}")]
        public async Task<IActionResult> UnassignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Status = "Error", Message = "User not found" });

            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
                return BadRequest(new { Status = "Error", Message = $"Role '{roleName}' does not exist" });

            // Check if user has the role
            if (!await _userManager.IsInRoleAsync(user, roleName))
                return BadRequest(new { Status = "Error", Message = $"User does not have the role '{roleName}'" });

            // Remove the role
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest(new { Status = "Error", Message = "Failed to remove role", Errors = result.Errors });

            if (roleName == "Dealer")
            {
                user.IsDealerVerified = false;

                // Delete dealer record if exists
                var dealerRecord = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (dealerRecord != null)
                {
                    _context.Dealers.Remove(dealerRecord);
                    await _context.SaveChangesAsync();
                }
            }

            if (roleName == "Manufacturer")
            {
                user.IsManufacturerVerified = false;
                // (If you also have a Manufacturers table, do the same cleanup here)
            }

            // Persist to AspNetUsers
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return BadRequest(new { Status = "Error", Message = "Failed to update user", Errors = updateResult.Errors });

            return Ok(new { Status = "Success", Message = $"Role '{roleName}' has been removed from user {user.UserName}" });
        }

    }

  }






