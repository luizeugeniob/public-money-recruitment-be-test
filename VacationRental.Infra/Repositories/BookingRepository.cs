using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Infra.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingRepository(IDictionary<int, BookingViewModel> bookings)
        {
            _bookings = bookings;
        }

        public ResourceIdViewModel Add(BookingBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _bookings.Keys.Count + 1 };

            _bookings.Add(key.Id, new BookingViewModel
            {
                Id = key.Id,
                Nights = model.Nights,
                RentalId = model.RentalId,
                Start = model.Start.Date
            });

            return key;
        }

        public BookingViewModel Get(int bookingId)
        {
            if (!_bookings.TryGetValue(bookingId, out var bookingViewModel))
            {
                return null;
            }

            return bookingViewModel;
        }

        public IEnumerable<BookingViewModel> GetAll()
        {
            return _bookings.Values;
        }

        public IEnumerable<BookingViewModel> GetBookingsRentedFor(int rentalId)
        {
            return GetAll().Where(x => x.RentalId == rentalId);
        }
    }
}