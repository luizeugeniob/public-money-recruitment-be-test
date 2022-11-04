using VacationRental.Domain.Models;

namespace VacationRental.Application.Interfaces
{
    public interface ICalendarAppService
    {
        CalendarViewModel Get(int rentalId, DateTime start, int nights);

        bool HasAtLeastOneUnoccupiedUnitPerNight(int rentalId, DateTime start, int nights);
    }
}