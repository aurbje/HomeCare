using HomeCare.Models;

namespace HomeCare.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Appointment>> GetAllBookingsAsync();
        Task<Appointment?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Appointment appointment);
        Task UpdateBookingAsync(Appointment appointment);
        Task DeleteBookingAsync(int id);
    }
}
