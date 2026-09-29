using DressRental.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DressRental.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}
