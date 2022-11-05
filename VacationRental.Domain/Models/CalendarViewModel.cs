using VacationRental.Domain.Models.Interfaces;

namespace VacationRental.Domain.Models
{
    public class CalendarViewModel : ICalendarViewModel
    {
        public int RentalId { get; set; }
        public List<CalendarDateViewModel> Dates { get; set; } = new List<CalendarDateViewModel>();

        public IEnumerable<int> GetUnitsOccupiedAt(DateTime date)
        {
            if (Dates.Count == 0)
            {
                return Enumerable.Empty<int>();
            }

            var calendarForSpecificDate = Dates.First(x => x.Date.Date.Equals(date.Date));

            return calendarForSpecificDate.Bookings.Select(x => x.Unit).Concat(calendarForSpecificDate.PreparationTimes.Select(x => x.Unit));
        }

        public bool HasOverbooking(int units)
        {
            return Dates.Any(x => x.Bookings.Count + x.PreparationTimes.Count > units);
        }

        public bool HasUnoccupiedUnitsAllDays(int units)
        {
            return Dates.All(x => x.Bookings.Count + x.PreparationTimes.Count < units);
        }
    }
}