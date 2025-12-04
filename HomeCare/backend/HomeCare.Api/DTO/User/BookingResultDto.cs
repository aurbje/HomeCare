using System.Collections.Generic;
using HomeCare.Api.Enums;

namespace HomeCare.Api.DTO.User 
{
    public class BookingResultDto // Result class for booking operations.
    {
        public bool Success { get; set; }
        public int? BookingId { get; set; }
        public string? Message { get; set; }
        public BookingResultType ResultType { get; set; }
        public Dictionary<string, string>? ValidationErrors { get; set; }
    }
}
