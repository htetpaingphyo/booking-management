using BookingManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BookingManagement.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceDescriptors(this IServiceCollection services)
        {
            foreach (var service in ServiceCollector.GetServices())
            {
                services.Add(service);
            }

            // Singleton DI for session
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Singleton DI for security
            services.AddSingleton<ISecurityService, SecurityService>();

            return services;
        }
    }
}
