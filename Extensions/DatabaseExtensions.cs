using Microsoft.EntityFrameworkCore;
using SellinBE.Models;

namespace SellinBE.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddProductionDb(this IServiceCollection services, IConfiguration config)
        {
            var ProdCnnString = config.GetConnectionString("ProductionDB") ?? throw new InvalidOperationException("Connection string not found.");

            services.AddDbContext<SellinProductionContext>(options =>
                options.UseSqlServer(ProdCnnString)
                );

            return services;
        }
    }
}
