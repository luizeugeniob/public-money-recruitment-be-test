using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services;

public class RentalAppService : IRentalAppService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICalendarAppService _calendarAppService;

    public RentalAppService(
        IRentalRepository rentalRepository,
        ICalendarAppService calendarAppService)
    {
        _rentalRepository = rentalRepository;
        _calendarAppService = calendarAppService;
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

    public ResourceIdViewModel Put(int rentalId, RentalBindingModel model)
    {
        if (!_rentalRepository.Exists(rentalId))
        {
            throw new RentalNotFoundException();
        }

        var simulatedCalendar = _calendarAppService.SimulateCalendar(rentalId, model.PreparationTimeInDays);
        if (simulatedCalendar.HasOverbooking(model.Units))
        {
            throw new UpdateWillCauseOverbookingException();
        }

        return _rentalRepository.Update(rentalId, model);
    }
}