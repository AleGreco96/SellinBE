namespace SellinBE.Extensions
{
    public static class CorsExtensions
    {
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

        public static IApplicationBuilder UseMyCors(this IApplicationBuilder app)
        {
            app.UseCors("MyCors");
            return app;
        }
    }
}
