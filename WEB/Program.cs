using DAL.Factories;
using DAL.Interfaces;
namespace WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddTransient<IFactory<string>, ServicesFactory>();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=LogIn}/{id?}");

            app.Run();
        }
    }
}
