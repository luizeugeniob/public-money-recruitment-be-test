using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services
{
    public class BookingAppService : IBookingAppService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly ICalendarAppService _calendarAppService;

        public BookingAppService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository,
            ICalendarAppService calendarAppService)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _calendarAppService = calendarAppService;
        }

        public BookingViewModel Get(int bookingId)
        {
            var bookingViewModel = _bookingRepository.Get(bookingId);
            if (bookingViewModel is null)
                throw new BookingNotFoundException();

            return bookingViewModel;
        }

        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new NightsMustBePositiveException();

            var rental = _rentalRepository.Get(model.RentalId);
            if (rental is null)
                throw new RentalNotFoundException();

            var calendar = _calendarAppService.Get(model.RentalId, model.Start, model.Nights);
            if (!calendar.HasUnoccupiedUnitsAllDays(rental.Units))
            {
                throw new RentalNotAvailableException();
            }

            var unoccupiedUnit = _calendarAppService.GetUnoccupiedUnitForSpecificNight(model.RentalId, model.Start);

            return _bookingRepository.Add(model, unoccupiedUnit);
        }
    }
}