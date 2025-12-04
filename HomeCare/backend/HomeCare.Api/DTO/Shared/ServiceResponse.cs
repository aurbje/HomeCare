namespace HomeCare.Api.DTO.Shared
{
    // a generic service response class to standardize API responses
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } // indicates if the operation was successful
        public string Message { get; set; } = string.Empty; // optional message
        public T? Data { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = "") 
            => new() { Success = true, Data = data, Message = message };

        public static ServiceResponse<T> FailResponse(string message)
            => new() { Success = false, Message = message };
    }
}
