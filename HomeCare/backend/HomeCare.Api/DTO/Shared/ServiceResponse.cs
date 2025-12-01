namespace HomeCare.Api.DTO.Shared
{
    // Generic response wrapper for services and controllers
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static ServiceResponse<T> FailResponse(string message)
            => new() { Success = false, Message = message };
    }
}
