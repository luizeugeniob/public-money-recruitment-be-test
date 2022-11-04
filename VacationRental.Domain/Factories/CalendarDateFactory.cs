using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Domain.Factories;

public class CalendarDateFactory : ICalendarDateFactory
{
    private readonly ICalendarBookingFactory _calendarBookingFactory;

    public CalendarDateFactory(ICalendarBookingFactory calendarBookingFactory)
    {
        _calendarBookingFactory = calendarBookingFactory;
    }

    public List<CalendarDateViewModel> CreateCalendarDates(DateTime start, int nights, IEnumerable<BookingViewModel> bookings)
    {
        var calendarDates = new List<CalendarDateViewModel>();
        for (var i = 0; i < nights; i++)
        {
            var currentDate = start.Date.AddDays(i).Date;

            calendarDates.Add(new CalendarDateViewModel
            {
                Date = currentDate,
                Bookings = _calendarBookingFactory.CreateBookingsForCurrentDate(bookings, currentDate)
            });
        }

        return calendarDates;
    }
}