namespace VacationRental.Domain.Models.Interfaces;

public interface ICalendarViewModel
{
    int RentalId { get; set; }
    List<CalendarDateViewModel> Dates { get; set; }

    IEnumerable<int> GetUnitsOccupiedAt(DateTime date);

    bool HasOverbooking(int units);

    bool HasUnoccupiedUnitsAllDays(int units);
}