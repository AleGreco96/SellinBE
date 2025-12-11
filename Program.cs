using SellinBE.Extensions;
using SellinBE.Hubs;

namespace SellinBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Custom Services ---------------------------------
            builder.Services.AddMyCors();

            builder.Services.AddProductionDb(builder.Configuration);
            builder.Services.AddSecurityDb(builder.Configuration);

            builder.Services.AddJwtSettings(builder.Configuration);
            builder.Services.AddPolicies();
            builder.Services.AddCryptography();
            builder.Services.AddTokens();

            builder.Services.AddSignalR();
            builder.Services.AddChatbot();
            // ------------------------------------------------------

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

            // Use Custom App Extensions ----
            app.UseMyCors();
            app.UseMyHub();
            // ------------------------------

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
