using VacationRental.Domain.Models.Interfaces;

namespace VacationRental.Application.Interfaces
{
    public interface ICalendarAppService
    {
        ICalendarViewModel Get(int rentalId, DateTime start, int nights);

        int GetUnoccupiedUnitForSpecificNight(int rentalId, DateTime start);

        ICalendarViewModel SimulateCalendar(int rentalId, int newPreparationTimeInDays);
    }
}