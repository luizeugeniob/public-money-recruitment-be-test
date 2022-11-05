using AutoFixture;
using Bogus;
using System;
using System.Collections.Generic;
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
            .Setup(x => x.Get(rentalId))
            .Returns(new RentalViewModel());

        var start = _fixture.Create<DateTime>();
        var nights = _faker.Random.Int(min: 1);

        // Act
        var response = service.Get(rentalId, start, nights);

        // Assert
        Assert.NotNull(response);
        _bookingRepository.Verify(x => x.GetBookingsRentedFor(rentalId), Times.Once);
        _calendarDateFactory.Verify(x => x.CreateCalendarDates(start, nights, It.IsAny<IEnumerable<BookingViewModel>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void GivenANonExistentRental_WhenGetUnoccupiedUnitForSpecificNight_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Get(rentalId))
            .Returns((RentalViewModel)null);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.GetUnoccupiedUnitForSpecificNight(It.IsAny<int>(), It.IsAny<DateTime>()));
    }

    [Fact]
    public void GivenAExistentRental_WhenSimulateCalendar_ThenReturnCalendar()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);

        var nights = _faker.Random.Int(1, 999);
        var firstNight = DateTime.Now;

        _bookingRepository
            .Setup(x => x.GetBookingsRentedFor(rentalId))
            .Returns(new List<BookingViewModel>
            {
                new BookingViewModel { Start = firstNight },
                new BookingViewModel { Start = DateTime.Now, Nights = nights }
            });

        // Act
        var response = service.SimulateCalendar(rentalId, It.IsAny<int>());

        // Assert
        Assert.NotNull(response);
        _bookingRepository.Verify(x => x.GetBookingsRentedFor(rentalId), Times.Once);
        _calendarDateFactory.Verify(x => x.CreateCalendarDates(firstNight, nights, It.IsAny<IEnumerable<BookingViewModel>>(), It.IsAny<int>()), Times.Once);
    }

    private CalendarAppService BuildService()
    {
        return new CalendarAppService(
            _bookingRepository.Object,
            _rentalRepository.Object,
            _calendarDateFactory.Object);
    }
}