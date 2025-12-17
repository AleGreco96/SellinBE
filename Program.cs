using ChatLib.Extensions;
using SecurityLib.Extensions;
using SellinBE.Extensions;
using ErrorLib.Extensions;

namespace SellinBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // ----------------------------------------
            builder.Services.AddProductionDb(builder.Configuration);
            builder.Services.AddSecurityLib(builder.Configuration);
            builder.Services.AddErrorLib(builder.Configuration);
            builder.Services.AddChatLib();
            builder.Services.AddMyCors();
            // ----------------------------------------

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ----------------------------------------
            app.UseErrorLib();
            app.MapHub<HubExtensions>("/chathub");
            app.UseMyCors();
            // ----------------------------------------
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
