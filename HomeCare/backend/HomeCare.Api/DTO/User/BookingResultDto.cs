using System.Collections.Generic;
using HomeCare.Api.Enums;

namespace HomeCare.Api.DTO.User // namespace for User-related DTOs
{
    public class BookingResultDto
    {
        public bool Success { get; set; } // indicates if the booking was successful
        public int? BookingId { get; set; }
        public string? Message { get; set; } // additional message or information
        public BookingResultType ResultType { get; set; }
        public Dictionary<string, string>? ValidationErrors { get; set; } // key-value pairs for validation errors
    }
}
