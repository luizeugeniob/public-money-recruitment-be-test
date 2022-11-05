using VacationRental.Domain.Models;

namespace VacationRental.Domain.Interfaces;

public interface ICalendarPreparationTimesFactory
{
    List<CalendarPreparationTimeViewModel> CreatePreparationTimesForCurrentDate(IEnumerable<BookingViewModel> bookings, DateTime currentDate, int preparationTimeInDays);
}