using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services
{
    public class RentalAppService : IRentalAppService
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalAppService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        public RentalViewModel Get(int rentalId)
        {
            var rentalViewModel = _rentalRepository.Get(rentalId);
            if (rentalViewModel is null)
                throw new RentalNotFoundException();

            return rentalViewModel;
        }

        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            return _rentalRepository.Add(model);
        }
    }
}