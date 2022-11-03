using VacationRental.Domain.Models;

namespace VacationRental.Infra.Interfaces
{
    public interface IRentalRepository
    {
        ResourceIdViewModel Add(RentalBindingModel model);

        bool Exists(int rentalId);

        RentalViewModel Get(int rentalId);
    }
}