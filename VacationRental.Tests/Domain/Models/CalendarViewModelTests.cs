using AutoFixture;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Domain.Models;

namespace VacationRental.Tests.Domain.Models;

public class CalendarViewModelTests
{
    private readonly Faker _faker;
    private readonly Fixture _fixture;

    public CalendarViewModelTests()
    {
        _faker = new Faker();
        _fixture = new Fixture();
    }

    [Fact]
    public void GivenACalendarWithoutDates_WhenGetUnitsOccupied_ThenReturnEmptyList()
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>()
        };

        // Act
        var response = calendar.GetUnitsOccupiedAt(It.IsAny<DateTime>());

        // Assert
        Assert.Empty(response);
    }

    [Fact]
    public void GivenACalendarWithDates_WhenGetUnitsOccupied_ThenReturnListOfAllOccupiedUnits()
    {
        // Arrange
        var bookedUnit = _faker.Random.Int(1, 999);
        var inPreparationUnit = _faker.Random.Int(1000, 9999);

        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel
                {
                    Bookings = new List<CalendarBookingViewModel>
                    {
                        new CalendarBookingViewModel
                        {
                            Unit = bookedUnit
                        }
                    },
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>
                    {
                        new CalendarPreparationTimeViewModel
                        {
                            Unit = inPreparationUnit
                        }
                    }
                }
            }
        };

        // Act
        var response = calendar.GetUnitsOccupiedAt(It.IsAny<DateTime>());

        // Assert
        Assert.Contains(response, x => x.Equals(bookedUnit));
        Assert.Contains(response, x => x.Equals(inPreparationUnit));
    }

    [Theory]
    [InlineData(3, 2, 0)]
    [InlineData(3, 1, 0)]
    [InlineData(3, 0, 0)]
    [InlineData(3, 0, 1)]
    [InlineData(3, 0, 2)]
    public void GivenACalendar_WhenVerifyIfHasUnoccupiedUnitsAllDays_ThenReturnTrue(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            }
        };

        // Act
        var response = calendar.HasUnoccupiedUnitsAllDays(units);

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
    public void GivenACalendar_WhenVerifyIfHasUnoccupiedUnitsAllDays_ThenReturnFalse(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            }
        };

        // Act
        var response = calendar.HasUnoccupiedUnitsAllDays(units);

        // Assert
        Assert.False(response);
    }

    [Theory]
    [InlineData(3, 4, 0)]
    [InlineData(3, 3, 1)]
    [InlineData(3, 2, 2)]
    [InlineData(3, 1, 3)]
    [InlineData(3, 0, 4)]
    public void GivenACalendarWithOverbooksInAllNights_WhenVerifyIfHasOverbook_ThenReturnTrue(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            }
        };

        // Act
        var response = calendar.HasOverbooking(units);

        // Assert
        Assert.True(response);
    }

    [Theory]
    [InlineData(3, 4, 0)]
    [InlineData(3, 3, 1)]
    [InlineData(3, 2, 2)]
    [InlineData(3, 1, 3)]
    [InlineData(3, 0, 4)]
    public void GivenACalendarWithOverbooksInOneNight_WhenVerifyIfHasOverbook_ThenReturnTrue(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(units).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(units).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(units).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(units).ToList() },
            }
        };

        // Act
        var response = calendar.HasOverbooking(units);

        // Assert
        Assert.True(response);
    }

    [Theory]
    [InlineData(3, 3, 0)]
    [InlineData(3, 2, 0)]
    [InlineData(3, 1, 0)]
    [InlineData(3, 0, 0)]
    [InlineData(3, 0, 1)]
    [InlineData(3, 0, 2)]
    [InlineData(3, 0, 3)]
    [InlineData(3, 1, 1)]
    [InlineData(3, 1, 2)]
    [InlineData(3, 2, 1)]
    public void GivenACalendarWithoutOverbooks_WhenVerifyIfHasOverbook_ThenReturnFalse(int units, int bookings, int preparationTimes)
    {
        // Arrange
        var calendar = new CalendarViewModel
        {
            Dates = new List<CalendarDateViewModel>
            {
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
                new CalendarDateViewModel { Bookings = _fixture.CreateMany<CalendarBookingViewModel>(bookings).ToList(), PreparationTimes = _fixture.CreateMany<CalendarPreparationTimeViewModel>(preparationTimes).ToList() },
            }
        };

        // Act
        var response = calendar.HasOverbooking(units);

        // Assert
        Assert.False(response);
    }
}