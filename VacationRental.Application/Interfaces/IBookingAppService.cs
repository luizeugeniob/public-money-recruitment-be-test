using VacationRental.Domain.Models;

namespace VacationRental.Application.Interfaces;

public interface IBookingAppService
{
    BookingViewModel Get(int bookingId);

    ResourceIdViewModel Post(BookingBindingModel model);
}