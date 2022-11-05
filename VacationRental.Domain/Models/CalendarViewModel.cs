namespace VacationRental.Domain.Models
{
    public class CalendarViewModel
    {
        public int RentalId { get; set; }
        public List<CalendarDateViewModel> Dates { get; set; }

        public IEnumerable<int> GetUnitsOccupiedAt(DateTime date)
        {
            var calendarForSpecificDate = Dates.First(x => x.Date.Date.Equals(date.Date));

            return calendarForSpecificDate.Bookings.Select(x => x.Unit).Concat(calendarForSpecificDate.PreparationTimes.Select(x => x.Unit));
        }
    }
}