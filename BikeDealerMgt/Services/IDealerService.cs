using BikeDealerMgtAPI.Models;

namespace BikeDealerMgtAPI.Services
{
	public interface IDealerService
	{
		public Task<Dealer?> AddDealer(Dealer dealer);
		public Task<Dealer?> UpdateDealer(int id, Dealer dealer);
		public Task<int> DeleteDealer(int id);
		public Task<List<Dealer>> GetAllDealers();
		public Task<Dealer> FindDealerById(int id);
		public Task<List<Dealer>> FindDealerByName(string Name);
		
	}
}
