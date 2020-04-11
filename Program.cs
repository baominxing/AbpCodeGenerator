using ABPCodeGenerator.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace ABPCodeGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(builder =>
            {
                Console.WriteLine("ConfigureWebHost");
            })
            .ConfigureAppConfiguration(builder =>
            {
                Console.WriteLine("ConfigureAppConfiguration");
            })
            .ConfigureLogging(webBuilder =>
            {
                Console.WriteLine("ConfigureLogging");
            })
            .ConfigureServices(builder =>
            {
                Console.WriteLine("ConfigureServices");
            })
            .ConfigureHostConfiguration(webBuilder =>
            {
                Console.WriteLine("ConfigureHostConfiguration");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                Console.WriteLine("ConfigureWebHostDefaults");

                webBuilder.UseStartup<Startup>();

                //webBuilder.ConfigureServices(services =>
                //{
                //    Console.WriteLine("StartUp.ConfigureServices");

                //    services.AddControllersWithViews();

                //    services.AddTransient<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();
                //});

                //webBuilder.Configure(app =>
                //{
                //    Console.WriteLine("StartUp.Configure");

                //    app.UseHttpsRedirection();

                //    app.UseStaticFiles();

                //    //app.UseIdentity();

                //    app.UseRouting();

                //    app.UseCors(builder => builder.WithOrigins("https://localhost:5001/"));

                //    app.UseAuthorization();

                //    app.UseEndpoints(endpoints =>
                //    {
                //        endpoints.MapControllerRoute(
                //            name: "default",
                //            pattern: "{controller=Dashboard}/{action=Index}/{id?}");
                //    });
                //});
            })

            ;
    }
}
