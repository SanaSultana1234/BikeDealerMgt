namespace BikeDealerMgtAPI.Models
{
	public class ServiceResponse<T>
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public T? Data { get; set; }

		public static ServiceResponse<T> SuccessResult(T data, string message = "") =>
			new ServiceResponse<T> { Success = true, Message = message, Data = data };

		public static ServiceResponse<T> Failure(string message) =>
			new ServiceResponse<T> { Success = false, Message = message };
	}

}
