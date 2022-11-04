using AutoFixture;
using Bogus;
using VacationRental.Application.Services;
using VacationRental.Domain.Exceptions;
using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;

namespace VacationRental.Tests.Application.Services;

public class RentalAppServiceTests
{
    private readonly Faker _faker;
    private readonly Fixture _fixture;

    private readonly Mock<IRentalRepository> _rentalRepository;

    public RentalAppServiceTests()
    {
        _faker = new Faker();
        _fixture = new Fixture();

        _rentalRepository = new Mock<IRentalRepository>();
    }

    [Fact]
    public void GivenANonExistentRental_WhenGetReturnsNull_ThenThrowAnException()
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
    public void GivenAExistentRental_WhenGetReturnsRental_ThenReturnsRental()
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

    private RentalAppService BuildService()
    {
        return new RentalAppService(_rentalRepository.Object);
    }
}