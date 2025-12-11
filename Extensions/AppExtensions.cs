using SellinBE.Hubs;

namespace SellinBE.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseMyCors(this IApplicationBuilder app)
        {
            app.UseCors("MyCors");
            return app;
        }

        public static IApplicationBuilder UseMyHub(this WebApplication app)
        {
            app.MapHub<ChatHub>("/chathub");
            return app;
        }
    }
}
