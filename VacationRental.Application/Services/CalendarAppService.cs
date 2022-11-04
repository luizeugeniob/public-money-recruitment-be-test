using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services
{
    public class CalendarAppService : ICalendarAppService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly ICalendarDateFactory _calendarDateFactory;

        public CalendarAppService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository,
            ICalendarDateFactory calendarDateFactory)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _calendarDateFactory = calendarDateFactory;
        }

        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new NightsMustBePositiveException();

            if (!_rentalRepository.Exists(rentalId))
                throw new RentalNotFoundException();

            var bookings = _bookingRepository.GetBookingsRentedFor(rentalId);

            return new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = _calendarDateFactory.CreateCalendarDates(start, nights, bookings)
            };
        }

        public bool HasAtLeastOneUnoccupiedUnitPerNight(int rentalId, DateTime start, int nights)
        {
            var rental = _rentalRepository.Get(rentalId);
            if (rental is null)
                throw new RentalNotFoundException();

            return Get(rentalId, start, nights).Dates.All(x => x.Bookings.Count < rental.Units);
        }
    }
}