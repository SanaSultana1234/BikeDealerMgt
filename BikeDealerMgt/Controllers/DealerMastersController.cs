using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DealerMastersController : ControllerBase
	{
		private readonly IDealerMasterService _DealerMasterService;

		public DealerMastersController(IDealerMasterService dealerMasterService)
		{
			_DealerMasterService = dealerMasterService;
		}

		//api/dealermaster
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet]
		public async Task<IActionResult> GetAllDealerMasters()
		{
			var dealerMasters = await _DealerMasterService.GetAllDMs();
			if (dealerMasters == null || dealerMasters.Count == 0)
				return NoContent();

			return Ok(dealerMasters);
		}

		//api/dealermaster/{id}
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetDealerMasterById(int id)
		{
			var dealerMaster = await _DealerMasterService.FindDMById(id);
			if (dealerMaster == null)
				return NotFound($"Dealer Master with ID {id} not found.");

			return Ok(dealerMaster);
		}

		//api/dealermasters/bike?name=xyz
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet("bike")]
		public async Task<IActionResult> GetDealersByBikeName([FromQuery] string name)
		{
			var dealerMasters = await _DealerMasterService.FindDealersByBikeName(name);
			return Ok(dealerMasters);
		}

		//api/dealermasters/dealer?name=xyz
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet("dealer")]
		public async Task<IActionResult> GetBikesByDealerName([FromQuery] string name)
		{
			var dealerMasters = await _DealerMasterService.FindBikesByDealerName(name);
			return Ok(dealerMasters);
		}


		//api/dealermaster
		[Authorize(Roles = "Dealer,Admin,Manufacturer")]
		[HttpPost]
		public async Task<IActionResult> AddDealerMaster([FromBody] DealerMaster dealerMaster)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _DealerMasterService.AddDM(dealerMaster);
			if (result == null) return BadRequest("Unable to create new Dealer Master. Try again later.");
			return CreatedAtAction(nameof(GetDealerMasterById), new { id = result.DealerMasterId }, result);
		}

		//api/dealermaster/{id}
		[Authorize(Roles = "Dealer,Admin,Manufacturer")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDealerMaster(int id, [FromBody] DealerMaster dealerMaster)
		{
			if (!ModelState.IsValid || dealerMaster == null || dealerMaster.DealerMasterId != id)
				return BadRequest("Invalid dealer master data.");

			var result = await _DealerMasterService.UpdateDM(id, dealerMaster);
			if (result == null)
				return NotFound($"Dealer Master with ID {id} not found.");

			return Ok(result);
		}

		// api/dealermaster/{id}
		[Authorize(Roles = "Dealer,Admin,Manufacturer")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDealerMaster(int id)
		{
			var result = await _DealerMasterService.DeleteDM(id);
			if (result==0)
				return NotFound($"Dealer Master with ID {id} not found.");

			return NoContent();
		}
	}
}
