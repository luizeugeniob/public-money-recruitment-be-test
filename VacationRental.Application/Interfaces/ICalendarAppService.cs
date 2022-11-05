using VacationRental.Domain.Models;

namespace VacationRental.Application.Interfaces
{
    public interface ICalendarAppService
    {
        CalendarViewModel Get(int rentalId, DateTime start, int nights);

        int GetUnoccupiedUnitForSpecificNight(int rentalId, DateTime start);

        bool HasAtLeastOneUnoccupiedUnitPerNight(int rentalId, DateTime start, int nights);
    }
}