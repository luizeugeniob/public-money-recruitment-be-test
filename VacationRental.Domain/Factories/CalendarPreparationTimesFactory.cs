using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;

namespace VacationRental.Domain.Factories;

public class CalendarPreparationTimesFactory : ICalendarPreparationTimesFactory
{
    public List<CalendarPreparationTimeViewModel> CreatePreparationTimesForCurrentDate(IEnumerable<BookingViewModel> bookings, DateTime currentDate, int preparationTimeInDays)
    {
        return bookings
            .Where(x => x.Start.AddDays(x.Nights) <= currentDate && x.Start.AddDays(x.Nights + preparationTimeInDays) > currentDate)
            .Select(x => new CalendarPreparationTimeViewModel { Unit = x.Unit })
            .ToList();
    }
}