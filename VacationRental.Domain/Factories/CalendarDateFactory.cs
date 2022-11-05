using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Domain.Factories;

public class CalendarDateFactory : ICalendarDateFactory
{
    private readonly ICalendarBookingFactory _calendarBookingFactory;
    private readonly ICalendarPreparationTimesFactory _calendarPreparationTimesFactory;

    public CalendarDateFactory(
        ICalendarBookingFactory calendarBookingFactory,
        ICalendarPreparationTimesFactory calendarPreparationTimesFactory)
    {
        _calendarBookingFactory = calendarBookingFactory;
        _calendarPreparationTimesFactory = calendarPreparationTimesFactory;
    }

    public List<CalendarDateViewModel> CreateCalendarDates(DateTime start, int nights, IEnumerable<BookingViewModel> bookings, int preparationTimeInDays)
    {
        var calendarDates = new List<CalendarDateViewModel>();

        for (var i = 0; i < nights; i++)
        {
            var currentDate = start.Date.AddDays(i).Date;

            calendarDates.Add(new CalendarDateViewModel
            {
                Date = currentDate,
                Bookings = _calendarBookingFactory.CreateBookingsForCurrentDate(bookings, currentDate),
                PreparationTimes = _calendarPreparationTimesFactory.CreatePreparationTimesForCurrentDate(bookings, currentDate, preparationTimeInDays)
            });
        }

        return calendarDates;
    }
}