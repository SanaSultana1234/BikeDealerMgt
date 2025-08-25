using BikeDealerMgtAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Services
{
	public class DealerService: IDealerService
	{
		private readonly BikeDealerMgmtDbContext _context;
        public DealerService(BikeDealerMgmtDbContext ctx)
        {
			_context = ctx;
		}
        public async Task<Dealer?> AddDealer(Dealer dealer)
		{
			if (dealer == null) return null;
			_context.Dealers.Add(dealer);
			await _context.SaveChangesAsync();
			return dealer;
		}
		public async Task<Dealer?> UpdateDealer(int id, Dealer dealer)
		{
			var existingDealer = await _context.Dealers.FindAsync(id);	
			if(existingDealer == null) return null;

			existingDealer.DealerName = dealer.DealerName;
			existingDealer.Address = dealer.Address;
			existingDealer.City = dealer.City;
			existingDealer.State = dealer.State;
			existingDealer.ZipCode = dealer.ZipCode;
			existingDealer.StorageCapacity = dealer.StorageCapacity;
			existingDealer.Inventory = dealer.Inventory;

			
			await _context.SaveChangesAsync();
			return existingDealer;
		}
		public async Task<int> DeleteDealer(int id)
		{
			var dealer = await _context.Dealers.FindAsync(id);
			if(dealer == null) return 0;
			_context.Dealers.Remove(dealer);
			return await _context.SaveChangesAsync();
		}
		public async Task<List<Dealer>> GetAllDealers()
		{
			var dealers = await _context.Dealers.ToListAsync();
			return dealers;
		}

		public async Task<Dealer?> FindDealerById(int id)
		{
			var bike = await _context.Dealers.FindAsync(id);
			return bike;
		}
		public async Task<List<Dealer>> FindDealerByName(string Name)
		{
			return await _context.Dealers
						.Where(d => d.DealerName.Contains(Name))
						.ToListAsync();
		}
	}
}
