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

        // DealerMasterService.cs
        public async Task<int> GetDealerMasterCount()
        {
            return await _context.DealerMasters.CountAsync();
        }

        public async Task<DealerMaster?> AddDM(DealerMaster dm)
		{
			if (dm == null || dm.BikesDelivered == null || dm.BikesDelivered <= 0) return null;

			var dealer = await _context.Dealers.FindAsync(dm.DealerId);
			var bike = await _context.BikeStores.FindAsync(dm.BikeId);

			if (dealer == null || bike==null) return null; //bike or dealer not found

			int currentInventory = dealer.Inventory;
			int storageCapacity = dealer.StorageCapacity;

			if (currentInventory + dm.BikesDelivered.Value <= storageCapacity)
			{
				dealer.Inventory = currentInventory + dm.BikesDelivered.Value;

				_context.DealerMasters.Add(dm);
				_context.Dealers.Update(dealer);

				await _context.SaveChangesAsync();
				return dm;
			}

			return null; // Capacity exceeded
		}

		public async Task<DealerMaster?> UpdateDM(int id, DealerMaster dm)
		{
			if (dm == null || dm.BikesDelivered == null || dm.BikesDelivered <= 0) return null;

			var existingDM = await _context.DealerMasters.FindAsync(id);
			if (existingDM == null) return null;

			var dealer = await _context.Dealers.FindAsync(dm.DealerId);
			var bike = await _context.BikeStores.FindAsync(dm.BikeId);

			if (dealer == null || bike == null) return null; //bike or dealer not found

			int oldQty = existingDM.BikesDelivered ?? 0;
			int newQty = dm.BikesDelivered.Value;

			var oldDealer = await _context.Dealers.FindAsync(existingDM.DealerId);
			if (oldDealer == null) return null;

			var newDealer = await _context.Dealers.FindAsync(dm.DealerId);
			if (newDealer == null) return null;

			if (oldDealer.DealerId == newDealer.DealerId)
			{
				// Same dealer → just adjust the difference
				int diff = newQty - oldQty;
				int currentInventory = oldDealer.Inventory;
				int capacity = oldDealer.StorageCapacity;

				if (currentInventory + diff <= capacity)
				{
					oldDealer.Inventory = currentInventory + diff;

					existingDM.BikeId = dm.BikeId;
					existingDM.BikesDelivered = dm.BikesDelivered;
					existingDM.DeliveryDate = dm.DeliveryDate;

					_context.Dealers.Update(oldDealer);
					_context.DealerMasters.Update(existingDM);

					await _context.SaveChangesAsync();
					return existingDM;
				}
			}
			else
			{
				// Different dealers → remove from old, add to new
				int oldInventory = oldDealer.Inventory;
				int newInventory = newDealer.Inventory;
				int capacity = newDealer.StorageCapacity;

				if (newInventory + newQty <= capacity)
				{
					oldDealer.Inventory = Math.Max(0, oldInventory - oldQty);
					newDealer.Inventory = newInventory + newQty;

					existingDM.DealerId = dm.DealerId;
					existingDM.BikeId = dm.BikeId;
					existingDM.BikesDelivered = dm.BikesDelivered;
					existingDM.DeliveryDate = dm.DeliveryDate;

					_context.Dealers.Update(oldDealer);
					_context.Dealers.Update(newDealer);
					_context.DealerMasters.Update(existingDM);

					await _context.SaveChangesAsync();
					return existingDM;
				}
			}

			return null; // Capacity exceeded
		}

		public async Task<int> DeleteDM(int id)
		{
			var dm = await _context.DealerMasters.FindAsync(id);
			if (dm == null || dm.BikesDelivered == null) return 0;

			var dealer = await _context.Dealers.FindAsync(dm.DealerId);
			if (dealer == null) return 0;

			dealer.Inventory = Math.Max(0, (dealer.Inventory) - dm.BikesDelivered.Value);

			_context.DealerMasters.Remove(dm);
			_context.Dealers.Update(dealer);

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
						.Distinct()
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
						.Distinct()
						.ToListAsync();
			return bikes;
		}
	}
}
