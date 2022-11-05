using VacationRental.Domain.Models;

namespace VacationRental.Domain.Interfaces;

public interface ICalendarDateFactory
{
    List<CalendarDateViewModel> CreateCalendarDates(DateTime start, int nights, IEnumerable<BookingViewModel> bookings, int preparationTimeInDays);
}