using BikeDealerMgtAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Services
{
	public class BikeService: IBikeService
	{
		private readonly BikeDealerMgmtDbContext _context;
		public BikeService(BikeDealerMgmtDbContext ctx) { 
			_context = ctx;
		}
		public async Task<BikeStore?> AddBike(BikeStore bike)
		{
			if (bike == null) return null;
			_context.BikeStores.Add(bike);
			int result = await _context.SaveChangesAsync();
			if(result == 0) return null;
			return bike;
		}
		public async Task<BikeStore?> UpdateBike(int id, BikeStore bike)
		{
			var existingBike = await _context.BikeStores.FindAsync(id);
			if (existingBike == null) return null;

			existingBike.ModelName = bike.ModelName;
			existingBike.ModelYear = bike.ModelYear;
			existingBike.EngineCc = bike.EngineCc;
			existingBike.Manufacturer = bike.Manufacturer;

			int result = await _context.SaveChangesAsync();
			if (result == 0) return null;
			return existingBike;
		}
		public async Task<int> DeleteBike(int id)
		{
			var bike = await _context.BikeStores.FindAsync(id);
			if (bike == null) return 0;

			_context.BikeStores.Remove(bike);
			return await _context.SaveChangesAsync();
		}
		public async Task<List<BikeStore>> GetAllBikes()
		{
			//var bikes = await _context.BikeStores.Include(b=>b.DealerMasters).ThenInclude(dm => dm.Dealer).ToListAsync();
			var bikes = await _context.BikeStores.ToListAsync();
			return bikes;
		}
		public async Task<BikeStore?> FindBikeById(int id)
		{
			var bike = await _context.BikeStores.FindAsync(id);
			return bike;
		}
		public async Task<List<BikeStore>> FindBikeByName(string name)
		{
			return await _context.BikeStores
						.Where(b => b.ModelName.ToLower().Contains(name.ToLower()))
						.ToListAsync();
		}
	}
}
