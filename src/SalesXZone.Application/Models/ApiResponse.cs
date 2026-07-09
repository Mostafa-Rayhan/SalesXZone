namespace SalesXZone.Application.Models
{
    public class ApiResponse<T>
    {
        public string SuccessCode { get; set; }
        public string SuccessMessage { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success", string code = "2000")
            => new ApiResponse<T> { SuccessCode = code, SuccessMessage = message, Data = data };

        public static ApiResponse<T> Fail(string message, string code = "5000", T data = default)
            => new ApiResponse<T> { SuccessCode = code, SuccessMessage = message, Data = data };
    }
}
