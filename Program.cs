using SellinBE.Extensions;
using Serilog;
using Serilog.Sinks.MSSqlServer;

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

            // PROVA CON SERILOG

            string logDbConnectionString = builder.Configuration.GetConnectionString("SecurityDB") ?? throw new InvalidOperationException("Connection string not found.");

            //GIOCHIAMO CON SERILOG
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console()
                .WriteTo.MSSqlServer(connectionString: logDbConnectionString, sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "ErrorLog",
                    AutoCreateSqlTable = true,
                    SchemaName = "dbo",
                })
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
            

            //
            /*Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console()
                        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();*/
            //

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //PROVIAMO A USARE ERRORMANAGER
            app.UseMiddleware<ErrorManager>();
            //

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
