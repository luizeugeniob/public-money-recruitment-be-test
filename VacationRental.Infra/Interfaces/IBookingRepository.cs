using VacationRental.Domain.Models;

namespace VacationRental.Infra.Interfaces
{
    public interface IBookingRepository
    {
        ResourceIdViewModel Add(BookingBindingModel model);

        BookingViewModel Get(int bookingId);

        IEnumerable<BookingViewModel> GetAll();

        IEnumerable<BookingViewModel> GetBookingsRentedFor(int rentalId);
    }
}