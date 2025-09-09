using BikeDealerMgtAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Services
{
	public class DealerService: IDealerService
	{
		private readonly BikeDealerMgmtDbContext _context;
		private readonly UserManager<AuthUser> _userManager;
		public DealerService(BikeDealerMgmtDbContext ctx, UserManager<AuthUser> userManager)
		{
			_context = ctx;
			_userManager = userManager;
		}

        // DealerService.cs
        public async Task<int> GetDealerCount()
        {
            return await _context.Dealers.CountAsync();
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
			if (dealer == null) return 0;

			var dealerMaster = await _context.DealerMasters.FirstOrDefaultAsync(dm => dm.DealerId == id);
			if (dealerMaster != null) return -1;

			// 1. Remove dealer from context first
			_context.Dealers.Remove(dealer);
			await _context.SaveChangesAsync(); // Commit dealer deletion first

			// 2. Now delete the associated user
			var user = await _userManager.FindByIdAsync(dealer.UserId);
			if (user == null) return 0;

			var roles = await _userManager.GetRolesAsync(user);
			if (roles.Any())
			{
				var roleResult = await _userManager.RemoveFromRolesAsync(user, roles);
				if (!roleResult.Succeeded) return 0;
			}

			var delResult = await _userManager.DeleteAsync(user);
			if (!delResult.Succeeded)
			{
				foreach (var error in delResult.Errors)
				{
					Console.WriteLine($"Error: {error.Code} - {error.Description}");
				}
				return 0;
			}

			return 1;
		}


		public async Task<List<Dealer>> GetAllDealers()
		{
			var dealers = await _context.Dealers.ToListAsync();
			return dealers;
		}

		public async Task<Dealer?> FindDealerById(int id)
		{
			var dealer = await _context.Dealers.FindAsync(id);
			return dealer;
		}
		public async Task<List<Dealer>> FindDealerByName(string Name)
		{
			return await _context.Dealers
						.Where(d => d.DealerName.ToLower().Contains(Name.ToLower()))
						.ToListAsync();
		}
	}
}
