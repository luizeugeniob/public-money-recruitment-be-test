using VacationRental.Domain.Models;
using VacationRental.Infra.Interfaces;
using VacationRental.Infra.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfra(this IServiceCollection services)
            => services
                .AddScoped<IRentalRepository, RentalRepository>()
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddFakeDatabase();

        private static IServiceCollection AddFakeDatabase(this IServiceCollection services)
            => services
                .AddSingleton<IDictionary<int, RentalViewModel>>(new Dictionary<int, RentalViewModel>())
                .AddSingleton<IDictionary<int, BookingViewModel>>(new Dictionary<int, BookingViewModel>());
    }
}