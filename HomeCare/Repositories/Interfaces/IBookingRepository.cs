using HomeCare.Models;

namespace HomeCare.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();   
        Task<Booking?> GetBookingByIdAsync(int id);         // get a booking  by id
        Task AddBookingAsync(Booking booking);              // add a new booking
        Task UpdateBookingAsync(Booking booking);           // update existing booking
        Task DeleteBookingAsync(int id);                    // delete a booking by id
    }
}
