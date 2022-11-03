﻿using VacationRental.Application.Interfaces;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Application.Services
{
    public class BookingAppService : IBookingAppService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;

        public BookingAppService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
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

            if (!_rentalRepository.Exists(model.RentalId))
                throw new RentalNotFoundException();

            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in _bookingRepository.GetAll())
                {
                    if (booking.RentalId == model.RentalId
                        && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                        || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                        || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= _rentalRepository.Get(model.RentalId)?.Units)
                    throw new RentalNotAvailableException();
            }

            return _bookingRepository.Add(model);
        }
    }
}