using BikeDealerMgtAPI.Models;

namespace BikeDealerMgtAPI.Services
{
	public interface IDealerMasterService
	{
        public Task<int> GetDealerMasterCount();
        public Task<DealerMaster?> AddDM(DealerMaster dm);
		public Task<DealerMaster?> UpdateDM(int id, DealerMaster dm);
		public Task<int> DeleteDM(int id);
		public Task<List<DealerMaster>> GetAllDMs();
		public Task<DealerMaster?> FindDMById(int id);
		public Task<List<Dealer>> FindDealersByBikeName(string name);
		public Task<List<BikeStore>> FindBikesByDealerName(string name);
	}
}
