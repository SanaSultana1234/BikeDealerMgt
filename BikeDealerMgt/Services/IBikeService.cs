using BikeDealerMgtAPI.Models;

namespace BikeDealerMgtAPI.Services
{
	public interface IBikeService
	{
        public Task<int> GetBikeCount();
        public Task<BikeStore> AddBike(BikeStore bike);
		public Task<BikeStore> UpdateBike(int id, BikeStore bike);
		public Task<int> DeleteBike(int id);
		public Task<List<BikeStore>> GetAllBikes();
		public Task<BikeStore> FindBikeById(int id);
		public Task<List<BikeStore>> FindBikeByName(string name);
	}
}
