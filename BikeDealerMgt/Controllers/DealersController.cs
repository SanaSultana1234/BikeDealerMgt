using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DealersController : ControllerBase
	{
		private readonly IDealerService _dealerService;

		public DealersController(IDealerService dealerService)
		{
			_dealerService = dealerService;
		}

		//api/dealer
		[HttpGet]
		public async Task<IActionResult> GetAllDealers()
		{
			var dealers = await _dealerService.GetAllDealers();
			if (dealers == null || dealers.Count == 0)
				return NoContent();

			return Ok(dealers);
		}

		//api/dealer/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetDealerById(int id)
		{
			var dealer = await _dealerService.FindDealerById(id);
			if (dealer == null)
				return NotFound($"Dealer with ID {id} not found.");

			return Ok(dealer);
		}

		//api/dealer/search?name=xyz
		[HttpGet("search")]
		public async Task<IActionResult> GetDealersByName([FromQuery] string name)
		{
			var dealers = await _dealerService.FindDealerByName(name);
			return Ok(dealers);
		}

		//api/dealer
		[HttpPost]
		public async Task<IActionResult> AddDealer([FromBody] Dealer dealer)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _dealerService.AddDealer(dealer);
			if (result == null) return BadRequest("Unable to create new Dealer. Try again later.");
			return CreatedAtAction(nameof(GetDealerById), new { id = result.DealerId }, result);
		}

		//api/dealer/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDealer(int id, [FromBody] Dealer dealer)
		{
			if (!ModelState.IsValid || dealer == null || dealer.DealerId != id)
				return BadRequest("Invalid dealer data.");

			var result = await _dealerService.UpdateDealer(id, dealer);
			if (result == null)
				return NotFound($"Dealer with ID {id} not found.");

			return Ok(result);
		}

		//api/dealer/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDealer(int id)
		{
			var result = await _dealerService.DeleteDealer(id);
			if (result ==0)
				return NotFound($"Dealer with ID {id} not found.");

			return NoContent();
		}
	}
}
