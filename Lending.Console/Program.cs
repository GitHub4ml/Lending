using Lending.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lending
{
    class Program
    {
        static void Main(string[] args)
        {
            //Simulate Dependency inversion to register services
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ILendingService, LendingService>();
                })
                .Build();

            var svc = ActivatorUtilities.CreateInstance<LendingService>(host.Services);
            svc.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("data.json", optional: false, reloadOnChange: true) //Simulate database like EF Core
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
