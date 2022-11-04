using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Domain.Factories;

public class CalendarBookingFactory : ICalendarBookingFactory
{
    public List<CalendarBookingViewModel> CreateBookingsForCurrentDate(IEnumerable<BookingViewModel> bookings, DateTime currentDate)
    {
        return bookings
            .Where(x => x.Start <= currentDate && x.Start.AddDays(x.Nights) > currentDate)
            .Select(x => new CalendarBookingViewModel { Id = x.Id })
            .ToList();
    }
}