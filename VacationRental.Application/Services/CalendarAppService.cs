using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;
using VacationRental.Domain.Models.Interfaces;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services
{
    public class CalendarAppService : ICalendarAppService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICalendarDateFactory _calendarDateFactory;
        private readonly IRentalRepository _rentalRepository;

        public CalendarAppService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository,
            ICalendarDateFactory calendarDateFactory)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _calendarDateFactory = calendarDateFactory;
        }

        public ICalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new NightsMustBePositiveException();

            var rental = _rentalRepository.Get(rentalId);
            if (rental is null)
                throw new RentalNotFoundException();

            var bookings = _bookingRepository.GetBookingsRentedFor(rentalId);

            return CreateCalendar(rentalId, start, nights, bookings, rental.PreparationTimeInDays);
        }

        public int GetUnoccupiedUnitForSpecificNight(int rentalId, DateTime start)
        {
            var rental = _rentalRepository.Get(rentalId);
            if (rental is null)
                throw new RentalNotFoundException();

            var calendar = Get(rentalId, start, 1);

            var units = calendar.GetUnitsOccupiedAt(start);

            return rental.Units.RandomIntExcept(units);
        }

        public ICalendarViewModel SimulateCalendar(int rentalId, int newPreparationTimeInDays)
        {
            var bookings = _bookingRepository.GetBookingsRentedFor(rentalId);
            if (!bookings.Any())
            {
                return new CalendarViewModel();
            }

            var firstNight = GetFirstOccupiedNight(bookings);
            var lastNight = GetLastOccupiedNight(bookings);
            var nights = firstNight.DifferenceInDaysFor(lastNight);

            return CreateCalendar(rentalId, firstNight, nights, bookings, newPreparationTimeInDays);
        }

        private static DateTime GetFirstOccupiedNight(IEnumerable<BookingViewModel> bookings)
        => bookings.OrderBy(x => x.Start).Select(x => x.Start).First();

        private static DateTime GetLastOccupiedNight(IEnumerable<BookingViewModel> bookings)
            => bookings.Select(x => x.Start.AddDays(x.Nights)).OrderByDescending(x => x).First();

        private ICalendarViewModel CreateCalendar(int rentalId, DateTime start, int nights, IEnumerable<BookingViewModel> bookings, int preparationTimeInDays)
        {
            return new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = _calendarDateFactory.CreateCalendarDates(start, nights, bookings, preparationTimeInDays)
            };
        }
    }
}