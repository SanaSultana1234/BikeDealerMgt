using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<AuthUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		public AuthController(UserManager<AuthUser> userManager,
							  RoleManager<IdentityRole> roleManager,
							  IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
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
				GSTNumber = model.GSTNumber
			};

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Dealer creation failed!", Errors = result.Errors });

			await EnsureRolesExist();
			await _userManager.AddToRoleAsync(user, UserRoles.Dealer);

			return Ok(new { Status = "Success", Message = "Dealer registered successfully" });
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
			await _userManager.AddToRoleAsync(user, UserRoles.Manufacturer);

			return Ok(new { Status = "Success", Message = "Manufacturer registered successfully" });
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

			var userRoles = await _userManager.GetRolesAsync(user);
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

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
