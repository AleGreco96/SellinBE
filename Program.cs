using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurityLib;
using SecurityLib.Models;
using SellinBE.Models;

namespace SellinBE
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add Tokens
            JwtSettings jwtSettings = new();
            jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
            builder.Services.AddSingleton(jwtSettings);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                });
            //

            // Add Policies
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireRole("Admin");
                });
                opt.AddPolicy("UserPolicy", policy =>
                {
                    policy.RequireRole("User", "Admin");
                });
            });
            //

            // Add Connection Strings
            var ProdCnnString = builder.Configuration.GetConnectionString("ProductionDB");
            var SecuCnnString = builder.Configuration.GetConnectionString("SecurityDB");

            if (string.IsNullOrEmpty(ProdCnnString))
                throw new InvalidOperationException("La connection string 'ProductionDB' non è definita nel file di configurazione.");

            if (string.IsNullOrEmpty(SecuCnnString))
                throw new InvalidOperationException("La connection string 'SecurityDB' non è definita nel file di configurazione.");
            //

            // Add Contexts
            builder.Services.AddDbContext<SellinProductionContext>(options =>
                options.UseSqlServer(ProdCnnString)
                );
            //

            // Add Services
            DatabaseService databaseService = new(SecuCnnString);
            builder.Services.AddSingleton(databaseService);

            builder.Services.AddSingleton<CryptographyService>();

            TokenService tokenService = new(jwtSettings);
            builder.Services.AddSingleton(tokenService);
            //

            // Add CORS Policies
            builder.Services.AddCors(
                opts => {
                    opts.AddPolicy("MyCors",
                    build => build
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(hosts => true));
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); 
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("MyCors"); //Add Cors Policies

            app.UseHttpsRedirection();
            //app.UseAuthentication(); //PROVIAMO PER I TOKEN DA SWAGGER
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
