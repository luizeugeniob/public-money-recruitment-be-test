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

    [Theory]
    [InlineData(3, 2, 0)]
    [InlineData(3, 1, 0)]
    [InlineData(3, 0, 0)]
    [InlineData(3, 0, 1)]
    [InlineData(3, 0, 2)]
    public void GivenAExistentRentalUnoccupied_WhenVerifyIfHasAtLeastOneUnoccupiedUnitPerNight_ThenReturnTrue(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var service = BuildService();

        _rentalRepository
            .Setup(x => x.Get(It.IsAny<int>()))
            .Returns(new RentalViewModel { Units = units });

        var dates = new List<CalendarDateViewModel>
        {
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
        };

        _calendarDateFactory
            .Setup(x => x.CreateCalendarDates(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<IEnumerable<BookingViewModel>>(), It.IsAny<int>()))
            .Returns(dates);

        // Act
        var response = service.HasAtLeastOneUnoccupiedUnitPerNight(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1));

        // Assert
        Assert.True(response);
    }

    /// <summary>
    /// A rental with 3 units is fully occupied for the selected night when it has (3 bookings) OR (2 bookings and 1 preparation time period) OR (1 booking and 2 preparation time periods) OR (3 preparation time periods).
    /// </summary>
    [Theory]
    [InlineData(3, 3, 0)]
    [InlineData(3, 2, 1)]
    [InlineData(3, 1, 2)]
    [InlineData(3, 0, 3)]
    public void GivenAExistentRentalOccupied_WhenVerifyIfHasAtLeastOneUnoccupiedUnitPerNight_ThenReturnFalse(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var service = BuildService();

        _rentalRepository
            .Setup(x => x.Get(It.IsAny<int>()))
            .Returns(new RentalViewModel { Units = units });

        var dates = new List<CalendarDateViewModel>
        {
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
        };

        _calendarDateFactory
            .Setup(x => x.CreateCalendarDates(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<IEnumerable<BookingViewModel>>(), It.IsAny<int>()))
            .Returns(dates);

        // Act
        var response = service.HasAtLeastOneUnoccupiedUnitPerNight(It.IsAny<int>(), It.IsAny<DateTime>(), _faker.Random.Int(min: 1));

        // Assert
        Assert.False(response);
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

    private CalendarAppService BuildService()
    {
        return new CalendarAppService(
            _bookingRepository.Object,
            _rentalRepository.Object,
            _calendarDateFactory.Object);
    }
}