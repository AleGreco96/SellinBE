using ChatLib;
using SecurityLib;
using SecurityLib.Models;

namespace SellinBE.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCryptography(this IServiceCollection services)
        {
            services.AddSingleton<CryptographyService>();
            return services;
        }
        
        public static IServiceCollection AddSecurityDb(this IServiceCollection services, IConfiguration config)
        {
            var SecuCnnString = config.GetConnectionString("SecurityDB");

            if (string.IsNullOrEmpty(SecuCnnString))
                throw new InvalidOperationException("The connection string 'SecurityDB' is not defined in the configuration file.");

            DatabaseService databaseService = new(SecuCnnString);
            services.AddSingleton(databaseService);
            return services;
        }

        public static IServiceCollection AddTokens(this IServiceCollection services)
        {
            services.AddSingleton<TokenService>();
            return services;
        }

        public static IServiceCollection AddChatbot(this IServiceCollection services)
        {
            services.AddSingleton<MessageService>();
            return services;
        }

        public static IServiceCollection AddMyCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyCors", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .SetIsOriginAllowed(_ => true);
                });
            });

            return services;
        }

    }
}
