using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [EnableCors]
    public class DealersController : ControllerBase
	{
		private readonly IDealerService _dealerService;

		public DealersController(IDealerService dealerService)
		{
			_dealerService = dealerService;
		}


		// Helper: Get DealerId from Claims
		private int? GetDealerIdFromClaims()
		{
			var dealerIdClaim = User.FindFirst("DealerId");
			if (dealerIdClaim != null && int.TryParse(dealerIdClaim.Value, out int dealerId))
				return dealerId;

			return null;
		}

        // DealersController.cs
        [Authorize(Roles = "User,Dealer,Admin,Manufacturer")]
        [HttpGet("count")]
        public async Task<IActionResult> GetDealerCount()
        {
            int count = await _dealerService.GetDealerCount();
            return Ok(count);
        }



        //api/dealers
        [HttpGet]
		[Authorize(Roles = "User,Dealer,Admin,Manufacturer")]
		//[AllowAnonymous]
		
		public async Task<IActionResult> GetAllDealers()
		{
			var dealers = await _dealerService.GetAllDealers();
			if (dealers == null || dealers.Count == 0)
				return NoContent();

			return Ok(dealers);
		}

		//api/dealer/{id}
		[Authorize(Roles = "User,Dealer,Admin,Manufacturer")]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetDealerById(int id)
		{
			var dealer = await _dealerService.FindDealerById(id);
			if (dealer == null)
				return NotFound($"Dealer with ID {id} not found.");

			return Ok(dealer);
		}

		//Below code is just for testing purpose
		//// GET: api/dealers/{id}
		//[Authorize(Roles = "User,Dealer,Admin,Manufacturer")]
		//[HttpGet("{id}")]
		//public async Task<IActionResult> GetDealerById(int id)
		//{
		//	var dealer = await _dealerService.FindDealerById(id);
		//	if (dealer == null)
		//		return NotFound($"Dealer with ID {id} not found.");

		//	if (User.IsInRole("Admin"))
		//		return Ok(dealer);

		//	if (User.IsInRole("Dealer"))
		//	{
		//		var dealerId = GetDealerIdFromClaims();
		//		if (dealerId == null || dealerId.Value != id)
		//			return Forbid();
		//		return Ok(dealer);
		//	}

		//	return Forbid();
		//}
		///////////////////////////////////////////////////

		//api/dealer/search?name=xyz
		//[Authorize(Roles = "User,Dealer,Admin,Manufacturer")]
		[HttpGet("search")]
		public async Task<IActionResult> GetDealersByName([FromQuery] string name)
		{
			var dealers = await _dealerService.FindDealerByName(name);
			return Ok(dealers);
		}

		//api/dealer
		[Authorize(Roles = "Dealer,Admin")]
		[HttpPost]
		public async Task<IActionResult> AddDealer([FromBody] Dealer dealer)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (User.IsInRole("Admin"))
			{
				var result = await _dealerService.AddDealer(dealer);
				return CreatedAtAction(nameof(GetDealerById), new { id = result.DealerId }, result);
			}

			if (User.IsInRole("Dealer"))
			{
				var dealerId = GetDealerIdFromClaims();
				if (dealerId != null)
					return BadRequest("Dealer already has a record."); // prevent duplicate dealers

				dealer.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // link to user
				var result = await _dealerService.AddDealer(dealer);
				return CreatedAtAction(nameof(GetDealerById), new { id = result.DealerId }, result);
			}

			return Forbid();
		}

		//api/dealer/{id}
		[Authorize(Roles = "Dealer,Admin")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDealer(int id, [FromBody] Dealer dealer)
		{
			if (dealer == null)
				return BadRequest("Invalid dealer data.");

			if (User.IsInRole("Admin"))
			{
				var result = await _dealerService.UpdateDealer(id, dealer);
				return result == null ? NotFound() : Ok(result);
			}
			if (User.IsInRole("Dealer"))
			{
				var dealerId = GetDealerIdFromClaims();
				if (dealerId == null || dealerId.Value != id)
					return Forbid();

				dealer.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var result = await _dealerService.UpdateDealer(id, dealer);
				return result == null ? NotFound() : Ok(result);
			}

			return Forbid();
		}

		// DELETE: api/dealers/{id}
		[Authorize(Roles = "Dealer,Admin")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDealer(int id)
		{
			if (User.IsInRole("Admin"))
			{
				var result = await _dealerService.DeleteDealer(id);

				if (result == 0)
					return NotFound($"Dealer with {id} not found");

				if (result == -1)
					return BadRequest("Cannot delete this dealer because it has bikes assigned.");

				return NoContent();
			}

			if (User.IsInRole("Dealer"))
			{
				var dealerId = GetDealerIdFromClaims();
				if (dealerId == null || dealerId.Value != id)
					return Forbid();

				var result = await _dealerService.DeleteDealer(id);
				if (result == -1) return BadRequest("Cannot delete this dealer because it has bikes assigned.");
				return result == 0 ? NotFound($"Dealer with {id} not found") : NoContent();
			}

			return Forbid();
		}
	}
}