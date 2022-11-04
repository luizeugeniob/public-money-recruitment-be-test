using AutoFixture;
using Bogus;
using System;
using VacationRental.Application.Interfaces;
using VacationRental.Application.Services;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Tests.Application.Services;

public class BookingAppServiceTests
{
    private readonly Faker _faker;
    private readonly Fixture _fixture;

    private readonly Mock<IBookingRepository> _bookingRepository;
    private readonly Mock<IRentalRepository> _rentalRepository;
    private readonly Mock<ICalendarAppService> _calendarAppService;

    public BookingAppServiceTests()
    {
        _faker = new Faker();
        _fixture = new Fixture();

        _bookingRepository = new Mock<IBookingRepository>();
        _rentalRepository = new Mock<IRentalRepository>();
        _calendarAppService = new Mock<ICalendarAppService>();
    }

    [Fact]
    public void GivenANonExistentId_WhenGetReturnsNull_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var bookingId = _faker.Random.Int(1, int.MaxValue);
        _bookingRepository
            .Setup(x => x.Get(bookingId))
            .Returns((BookingViewModel)null);

        // Act & Assert
        Assert.Throws<BookingNotFoundException>(() => service.Get(bookingId));
    }

    [Fact]
    public void GivenAExistentId_WhenGetReturnsNotNull_ThenReturnBooking()
    {
        // Arrange
        var service = BuildService();

        var bookingId = _faker.Random.Int(1, int.MaxValue);
        var booking = _fixture.Create<BookingViewModel>();
        _bookingRepository
            .Setup(x => x.Get(bookingId))
            .Returns(booking);

        // Act
        var response = service.Get(bookingId);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(booking, response);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenANonPositiveNight_WhenPostBooking_ThenThrowAnException(int nights)
    {
        // Arrange
        var service = BuildService();

        // Act & Assert
        Assert.Throws<NightsMustBePositiveException>(() => service.Post(new BookingBindingModel { Nights = nights }));
    }

    [Fact]
    public void GivenANonExistentRental_WhenPostBooking_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(false);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.Post(new BookingBindingModel { Nights = _faker.Random.Int(1, int.MaxValue), RentalId = rentalId }));
    }

    [Fact]
    public void GivenARentalTotalyOccupied_WhenPostBooking_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(true);

        _calendarAppService
            .Setup(x => x.HasAtLeastOneUnoccupiedUnitPerNight(rentalId, It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns(false);

        // Act & Assert
        Assert.Throws<RentalNotAvailableException>(() => service.Post(new BookingBindingModel { Nights = _faker.Random.Int(1, int.MaxValue), RentalId = rentalId }));
    }

    [Fact]
    public void GivenAUnoccupiedRental_WhenPostBooking_ThenAddBooking()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(true);

        _calendarAppService
            .Setup(x => x.HasAtLeastOneUnoccupiedUnitPerNight(rentalId, It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns(true);

        // Act
        service.Post(new BookingBindingModel { Nights = _faker.Random.Int(1, int.MaxValue), RentalId = rentalId });

        // Assert
        _bookingRepository.Verify(x => x.Add(It.IsAny<BookingBindingModel>()), Times.Once);
    }

    private BookingAppService BuildService()
    {
        return new BookingAppService(
            _bookingRepository.Object,
            _rentalRepository.Object,
            _calendarAppService.Object);
    }
}