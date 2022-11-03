using VacationRental.Application.Interfaces;
using VacationRental.Application.Models;

namespace VacationRental.Application.Services
{
    public class RentalAppService : IRentalAppService
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;

        public RentalAppService(IDictionary<int, RentalViewModel> rentals)
        {
            _rentals = rentals;
        }

        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.ContainsKey(rentalId))
                throw new ApplicationException("Rental not found");

            return _rentals[rentalId];
        }

        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units
            });

            return key;
        }
    }
}