using AutoFixture;
using Bogus;
using VacationRental.Application.Interfaces;
using VacationRental.Application.Services;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Domain.Models.Interfaces;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Tests.Application.Services;

public class RentalAppServiceTests
{
    private readonly Faker _faker;
    private readonly Fixture _fixture;

    private readonly Mock<IRentalRepository> _rentalRepository;
    private readonly Mock<ICalendarAppService> _calendarAppService;

    public RentalAppServiceTests()
    {
        _faker = new Faker();
        _fixture = new Fixture();

        _rentalRepository = new Mock<IRentalRepository>();
        _calendarAppService = new Mock<ICalendarAppService>();
    }

    [Fact]
    public void GivenANonExistentRental_WhenGet_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Get(rentalId))
            .Returns((RentalViewModel)null);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.Get(rentalId));
    }

    [Fact]
    public void GivenAExistentRental_WhenGet_ThenReturnsRental()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        var rental = _fixture.Create<RentalViewModel>();
        _rentalRepository
            .Setup(x => x.Get(rentalId))
            .Returns(rental);

        // Act
        var response = service.Get(rentalId);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(rental, response);
    }

    [Fact]
    public void GivenANonExistentRental_WhenPut_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(false);

        // Act & Assert
        Assert.Throws<RentalNotFoundException>(() => service.Put(rentalId, It.IsAny<RentalBindingModel>()));
    }

    [Fact]
    public void GivenACalendarOverbooked_WhenPut_ThenThrowAnException()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(true);

        var simulatedCalendar = new Mock<ICalendarViewModel>();
        _calendarAppService
            .Setup(x => x.SimulateCalendar(rentalId, It.IsAny<int>()))
            .Returns(simulatedCalendar.Object);

        simulatedCalendar
            .Setup(x => x.HasOverbooking(It.IsAny<int>()))
            .Returns(true);

        // Act & Assert
        Assert.Throws<UpdateWillCauseOverbookingException>(() => service.Put(rentalId, new RentalBindingModel()));
    }

    [Fact]
    public void GivenACalendarNonOverbooked_WhenPut_ThenUpdateRental()
    {
        // Arrange
        var service = BuildService();

        var rentalId = _faker.Random.Int(1, int.MaxValue);
        _rentalRepository
            .Setup(x => x.Exists(rentalId))
            .Returns(true);

        var simulatedCalendar = new Mock<ICalendarViewModel>();
        _calendarAppService
            .Setup(x => x.SimulateCalendar(rentalId, It.IsAny<int>()))
            .Returns(simulatedCalendar.Object);

        simulatedCalendar
            .Setup(x => x.HasOverbooking(It.IsAny<int>()))
            .Returns(false);

        // Act
        service.Put(rentalId, new RentalBindingModel());

        // Assert
        _rentalRepository.Verify(x => x.Update(rentalId, It.IsAny<RentalBindingModel>()), Times.Once);
    }

    private RentalAppService BuildService()
    {
        return new RentalAppService(
            _rentalRepository.Object,
            _calendarAppService.Object);
    }
}