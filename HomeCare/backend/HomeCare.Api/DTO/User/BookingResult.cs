using HomeCare.Api.Enums;

namespace HomeCare.Api.DTO
{
    /// Result class for booking operations.
    public class BookingResult
    {
        public bool Success { get; set; }
        public int? BookingId { get; set; }
        public string? Message { get; set; }
        public BookingResultType ResultType { get; set; }
        public Dictionary<string, string>? ValidationErrors { get; set; }
    }
}
