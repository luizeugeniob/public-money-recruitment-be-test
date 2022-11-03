﻿using VacationRental.Application.Models;

namespace VacationRental.Application.Interfaces
{
    public interface IRentalAppService
    {
        RentalViewModel Get(int rentalId);

        ResourceIdViewModel Post(RentalBindingModel model);
    }
}