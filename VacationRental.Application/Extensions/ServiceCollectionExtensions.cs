using VacationRental.Application.Interfaces;
using VacationRental.Application.Models;
using VacationRental.Application.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
            => services
                .AddScoped<IRentalAppService, RentalAppService>()
                .AddScoped<IBookingAppService, BookingAppService>()
                .AddScoped<ICalendarAppService, CalendarAppService>()
                .AddFakeDatabase();

        private static IServiceCollection AddFakeDatabase(this IServiceCollection services)
            => services
                .AddSingleton<IDictionary<int, RentalViewModel>>(new Dictionary<int, RentalViewModel>())
                .AddSingleton<IDictionary<int, BookingViewModel>>(new Dictionary<int, BookingViewModel>());
    }
}