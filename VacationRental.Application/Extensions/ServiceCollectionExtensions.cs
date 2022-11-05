using VacationRental.Application.Interfaces;
using VacationRental.Application.Services;
using VacationRental.Domain.Factories;
using VacationRental.Domain.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
        => services
            .AddScoped<IRentalAppService, RentalAppService>()
            .AddScoped<IBookingAppService, BookingAppService>()
            .AddScoped<ICalendarAppService, CalendarAppService>()
            .AddFactories();

    private static IServiceCollection AddFactories(this IServiceCollection services)
        => services
            .AddScoped<ICalendarDateFactory, CalendarDateFactory>()
            .AddScoped<ICalendarBookingFactory, CalendarBookingFactory>()
            .AddScoped<ICalendarPreparationTimesFactory, CalendarPreparationTimesFactory>();
}