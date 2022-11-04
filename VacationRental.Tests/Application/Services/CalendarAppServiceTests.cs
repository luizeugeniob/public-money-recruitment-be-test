using AutoFixture;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Application.Services;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Tests.Application.Services;

public class CalendarAppServiceTests
{
    private readonly Faker _faker;
    private readonly Fixture _fixture;

    private readonly Mock<IBookingRepository> _bookingRepository;
    private readonly Mock<IRentalRepository> _rentalRepository;
    private readonly Mock<ICalendarDateFactory> _calendarDateFactory;

    public CalendarAppServiceTests()
    {
        _faker = new Faker();
        _fixture = new Fixture();

        _bookingRepository = new Mock<IBookingRepository>();
        _rentalRepository = new Mock<IRentalRepository>();
        _calendarDateFactory = new Mock<ICalendarDateFactory>();
    }

    [Fact]
    public void GivenANegativeNight_WhenGetCalendar_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        // Act & Assert
        Assert.Throws<NightsMustBePositiveException>(() => service.Get(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(max: -1)));
    }

    [Fact]
    public void GivenANonExistentRental_WhenGetCalendar_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(false);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.Get(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1)));
    }

    [Fact]
    public void GivenAExistentRental_WhenGetCalendar_ThenReturnCalendar()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(true);

        var start = _fixture.Create<DateTime>();
        var nights = _faker.Random.Int(min: 1);

        // Act
        var response = service.Get(rentalId, start, nights);

        // Assert
        Assert.NotNull(response);
        _bookingRepository.Verify(x => x.GetBookingsRentedFor(rentalId), Times.Once);
        _calendarDateFactory.Verify(x => x.CreateCalendarDates(start, nights, It.IsAny<IEnumerable<BookingViewModel>>()), Times.Once);
    }

    [Fact]
    public void GivenANonExistentRental_WhenVerifyIfHasAtLeastOneUnoccupiedUnitPerNight_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Get(rentalId))
            .Returns((RentalViewModel)null);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.HasAtLeastOneUnoccupiedUnitPerNight(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1)));
    }

    [Fact]
    public void GivenAExistentRentalUnoccupied_WhenVerifyIfHasAtLeastOneUnoccupiedUnitPerNight_ThenReturnTrue()
    {
        // Arrange
        var service = BuildService();

        _rentalRepository
            .Setup(x => x.Exists(It.IsAny<int>()))
            .Returns(true);

        // Generate a rental with a random number of units
        var units = _faker.Random.Int(1, 99);
        _rentalRepository
            .Setup(x => x.Get(It.IsAny<int>()))
            .Returns(new RentalViewModel { Units = units });

        // Generates a list of dates when every night will have at least one unit unoccupied
        var dates = new List<CalendarDateViewModel>
        {
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(_faker.Random.Int(1, units - 1)).ToList() } , // At least one unoccupied unit
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(_faker.Random.Int(1, units - 1)).ToList() } , // At least one unoccupied unit
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(_faker.Random.Int(1, units - 1)).ToList() } , // At least one unoccupied unit
        };

        _calendarDateFactory
            .Setup(x => x.CreateCalendarDates(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<IEnumerable<BookingViewModel>>()))
            .Returns(dates);

        // Act
        var response = service.HasAtLeastOneUnoccupiedUnitPerNight(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1));

        // Assert
        Assert.True(response);
    }

    [Fact]
    public void GivenAExistentRentalOccupied_WhenVerifyIfHasAtLeastOneUnoccupiedUnitPerNight_ThenReturnFalse()
    {
        // Arrange
        var service = BuildService();

        _rentalRepository
            .Setup(x => x.Exists(It.IsAny<int>()))
            .Returns(true);

        // Generate a rental with a random number of units
        var units = _faker.Random.Int(1, 99);
        _rentalRepository
            .Setup(x => x.Get(It.IsAny<int>()))
            .Returns(new RentalViewModel { Units = units });

        // Generates a list of dates in which at least one of the nights will have all units rented
        var dates = new List<CalendarDateViewModel>
        {
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(units).ToList() } , // All units occupied
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(_faker.Random.Int(1, units -1)).ToList() } , // At least one unoccupied unit
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(_faker.Random.Int(1, units -1)).ToList() } , // At least one unoccupied unit
        };

        _calendarDateFactory
            .Setup(x => x.CreateCalendarDates(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<IEnumerable<BookingViewModel>>()))
            .Returns(dates);

        // Act
        var response = service.HasAtLeastOneUnoccupiedUnitPerNight(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1));

        // Assert
        Assert.False(response);
    }

    private CalendarAppService BuildService()
    {
        return new CalendarAppService(
            _bookingRepository.Object,
            _rentalRepository.Object,
            _calendarDateFactory.Object);
    }
}