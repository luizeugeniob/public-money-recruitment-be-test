using VacationRental.Application.Interfaces;
using VacationRental.Application.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
            => services
                .AddScoped<IRentalAppService, RentalAppService>()
                .AddScoped<IBookingAppService, BookingAppService>()
                .AddScoped<ICalendarAppService, CalendarAppService>();
    }
}