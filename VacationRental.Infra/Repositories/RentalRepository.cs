using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Infra.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;

        public RentalRepository(IDictionary<int, RentalViewModel> rentals)
        {
            _rentals = rentals;
        }

        public ResourceIdViewModel Add(RentalBindingModel model)
        {
            var key = new ResourceIdViewModel { Id = _rentals.Keys.Count + 1 };

            _rentals.Add(key.Id, new RentalViewModel
            {
                Id = key.Id,
                Units = model.Units
            });

            return key;
        }

        public bool Exists(int rentalId)
        {
            return _rentals.ContainsKey(rentalId);
        }

        public RentalViewModel Get(int rentalId)
        {
            if (!_rentals.TryGetValue(rentalId, out var rentalViewModel))
            {
                return null;
            }

            return rentalViewModel;
        }
    }
}