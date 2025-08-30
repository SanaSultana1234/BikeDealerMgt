using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeDealerMgtAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BikesController : ControllerBase
	{
		private readonly IBikeService _BikeService;
		
		public BikesController(IBikeService bs) {
			_BikeService = bs;
		}

		//api/bikes
		[Authorize(Roles="Customer,Dealer,Admin,Manufacturer")]
		[HttpGet]
		public async Task<IActionResult> GetBikes()
		{
			List<BikeStore> bikes = await _BikeService.GetAllBikes();
			if (bikes == null || bikes.Count == 0) return NoContent();
		
			return Ok(bikes);
		}

		//api/bike/{id}
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetBikeById(int id)
		{
			var bike = await _BikeService.FindBikeById(id);
			if(bike == null) return NotFound();
			
			return Ok(bike);
		}

		//api/bike/search?name=xyz
		[Authorize(Roles = "Customer,Dealer,Admin,Manufacturer")]
		[HttpGet("search")]
		public async Task<IActionResult> GetBikesByName([FromQuery] string name)
		{
			var bikes = await _BikeService.FindBikeByName(name);
			if (bikes == null) return NotFound();

			return Ok(bikes);
		}

		//api/bike
		[Authorize(Roles = "Admin,Manufacturer")]
		[HttpPost]
		public async Task<IActionResult> AddBike([FromBody] BikeStore bike)
		{
			if(!ModelState.IsValid) return BadRequest(ModelState);
			BikeStore? result = await _BikeService.AddBike(bike);
			if (result == null) return BadRequest("Unable to create new bike. Try again later.");
			return CreatedAtAction(nameof(GetBikeById), new {id=result.BikeId}, result);
		}

		//api/bike/{id}
		[Authorize(Roles = "Admin,Manufacturer")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBike(int id, [FromBody] BikeStore bike)
		{
			if (!ModelState.IsValid || bike == null || bike.BikeId!=id) return BadRequest("Invalid Bike Data");
			BikeStore? result = await _BikeService.UpdateBike(id, bike);
			if (result == null) return NotFound($"Bike with ID {id} not found.");
			return Ok(result);
		}

		//api/bike/{id}
		[Authorize(Roles = "Admin,Manufacturer")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBike(int id)
		{
			int result = await _BikeService.DeleteBike(id);
			if (result == 0) return NotFound();
			return NoContent();
		}


	}
}
