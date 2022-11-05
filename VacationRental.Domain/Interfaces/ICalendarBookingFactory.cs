using VacationRental.Domain.Models;

namespace VacationRental.Domain.Interfaces;

public interface ICalendarBookingFactory
{
    List<CalendarBookingViewModel> CreateBookingsForCurrentDate(IEnumerable<BookingViewModel> bookings, DateTime currentDate);
}