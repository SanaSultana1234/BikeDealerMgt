using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Microsoft.AspNetCore.Cors;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableCors]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<AuthUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		private readonly BikeDealerMgmtDbContext _db;
		
		public AuthController(UserManager<AuthUser> userManager,
							  RoleManager<IdentityRole> roleManager,
							  IConfiguration configuration, BikeDealerMgmtDbContext db)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_db = db;
		}

		// Register User
		[HttpPost("register-user")]
		public async Task<IActionResult> RegisterUser([FromBody] UserRegisterModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "Username already exists!" });

			var user = new AuthUser
			{
				UserName = model.Username,
				Email = model.Email,
				Address = model.Address
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed!", Errors = result.Errors });

			await EnsureRolesExist();
			await _userManager.AddToRoleAsync(user, UserRoles.User);

			return Ok(new { Status = "Success", Message = "User registered successfully" });
		}

		// Register Dealer
		[HttpPost("register-dealer")]
		public async Task<IActionResult> RegisterDealer([FromBody] DealerRegisterModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "Username already exists!" });

			var user = new AuthUser
			{
				UserName = model.Username,
				Email = model.Email,
				Address = model.Address,
				StoreName = model.StoreName,
				StorageCapacity = model.StorageCapacity,
				Inventory = model.Inventory
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Dealer creation failed!", Errors = result.Errors });

            await EnsureRolesExist();
            await _userManager.AddToRoleAsync(user, UserRoles.User); // Not Dealer yet

            return Ok(new { Status = "Pending", Message = "Dealer registration submitted for approval" });
		}

		//Register Manufacturer
		[HttpPost("register-manufacturer")]
		public async Task<IActionResult> RegisterManufacturer([FromBody] ManufacturerRegisterModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "Username already exists!" });

			var user = new AuthUser
			{
				UserName = model.Username,
				Email = model.Email,
				Address = model.Address,
				CompanyName = model.CompanyName,
				RegistrationNumber = model.RegistrationNumber
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Manufacturer creation failed!", Errors = result.Errors });

			await EnsureRolesExist();
			await _userManager.AddToRoleAsync(user, UserRoles.User); // Not Manufacturer yet

			return Ok(new { Status = "Pending", Message = "Manufacturer registration submitted for approval" });
		}

		// Login
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByNameAsync(model.Username);
			if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
				return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });

			//build auth claims to store username id, and dealer id if a dealer
			var authClaims = await BuildClaimsAsync(user);
			authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			//GetToken
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);
			//---------

			return Ok(new
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				Expiration = token.ValidTo
			});
		}

		private async Task<IList<Claim>> BuildClaimsAsync(AuthUser user)
		{
			var claims = new List<Claim>
	{
		new Claim("id", user.Id),
		new Claim("username", user.UserName ?? ""),
		new Claim("email", user.Email ?? "")
	};

			var roles = await _userManager.GetRolesAsync(user);
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
			claims.AddRange(roles.Select(r => new Claim("role", r)));

			// Attach DealerId if present
			var dealerId = await _db.Dealers
				.Where(d => d.UserId == user.Id)
				.Select(d => d.DealerId)
				.SingleOrDefaultAsync();

			if (dealerId > 0)
				claims.Add(new Claim("DealerId", dealerId.ToString()));

			return claims;
		}


		private async Task EnsureRolesExist()
		{
			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.Dealer))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Dealer));
			if (!await _roleManager.RoleExistsAsync(UserRoles.Manufacturer))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manufacturer));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
		}
	}
}
