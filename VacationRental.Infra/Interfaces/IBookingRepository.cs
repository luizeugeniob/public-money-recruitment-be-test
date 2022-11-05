using VacationRental.Domain.Models;

namespace VacationRental.Infra.Interfaces
{
    public interface IBookingRepository
    {
        ResourceIdViewModel Add(BookingBindingModel model, int unit);

        BookingViewModel Get(int bookingId);

        IEnumerable<BookingViewModel> GetBookingsRentedFor(int rentalId);
    }
}