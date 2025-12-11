using Microsoft.EntityFrameworkCore;
using SecurityLib;
using SellinBE.Models;

namespace SellinBE.Extensions
{
    public static class ContextExtensions
    {
        public static IServiceCollection AddProductionDb(this IServiceCollection services, IConfiguration config)
        {
            var ProdCnnString = config.GetConnectionString("ProductionDB");

            if (string.IsNullOrEmpty(ProdCnnString))
                throw new InvalidOperationException("The connection string 'ProductionDB' is not defined in the configuration file.");

            services.AddDbContext<SellinProductionContext>(options =>
                options.UseSqlServer(ProdCnnString)
                );

            return services;
        }
    }
}
