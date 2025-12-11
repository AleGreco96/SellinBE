using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SecurityLib.Models;

namespace SellinBE.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtSettings(this IServiceCollection services, IConfiguration config)
        {
            JwtSettings jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                        {
                            var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);
                            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtSettings.Issuer,
                                ValidAudience = jwtSettings.Audience,
                                IssuerSigningKey = new SymmetricSecurityKey(key)
                            };
                        }
                    );

            return services;
        }

        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireRole("User", "Admin");
                });
            });

            return services;
        }

    }
}
