using BikeDealerMgtAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Services
{
	public class DealerMasterService: IDealerMasterService
	{
		private readonly BikeDealerMgmtDbContext _context;
		public DealerMasterService(BikeDealerMgmtDbContext ctx)
		{
			_context = ctx;
		}
		public async Task<DealerMaster?> AddDM(DealerMaster dm)
		{
			if (dm == null) return null;
			_context.DealerMasters.Add(dm);
			await _context.SaveChangesAsync();
			return dm;
		}
		public async Task<DealerMaster?> UpdateDM(int id, DealerMaster dm)
		{
			var existingDealer = await _context.DealerMasters.FindAsync(id);
			if (existingDealer == null) return null;

			existingDealer.DealerId = dm.DealerId;
			existingDealer.BikeId = dm.BikeId;
			existingDealer.BikesDelivered = dm.BikesDelivered;
			existingDealer.DeliveryDate = dm.DeliveryDate;

			await _context.SaveChangesAsync();
			return existingDealer;
		}
		public async Task<int> DeleteDM(int id)
		{
			var dm = await _context.DealerMasters.FindAsync(id);
			if (dm == null) return 0;

			_context.DealerMasters.Remove(dm);
			return await _context.SaveChangesAsync();
		}
		public async Task<List<DealerMaster>> GetAllDMs()
		{
			var DMs = await _context.DealerMasters
									.Include(dm => dm.Bike)
									.Include(dm => dm.Dealer)
									.ToListAsync();
			return DMs;
		}
		public async Task<DealerMaster> FindDMById(int id)
		{
			var dm = await _context.DealerMasters
							.Include(dm => dm.Bike)
							.Include(dm => dm.Dealer)
							.FirstOrDefaultAsync(dm => dm.DealerMasterId==id);
			return dm;
		}
		public async Task<List<Dealer>> FindDealersByBikeName(string name)
		{
			var dealers = await _context.DealerMasters
						.Include(dm => dm.Bike)
						.Include(dm => dm.Dealer)
						.Where(dm => dm.Bike.ModelName.ToLower().Contains(name.ToLower()))
						.Select(dm => dm.Dealer)
						.ToListAsync();
			return dealers;
		}

		public async Task<List<BikeStore>> FindBikesByDealerName(string name)
		{
			var bikes = await _context.DealerMasters
						.Include(dm => dm.Bike)
						.Include(dm => dm.Dealer)
						.Where(dm => dm.Dealer.DealerName.ToLower().Contains(name.ToLower()))
						.Select(dm => dm.Bike)
						.ToListAsync();
			return bikes;
		}
	}
}
