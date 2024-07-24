using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Praktika_Worker_Service
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			

			try
			{
                var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => //permest i setup
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .SetupApplication(); // Ensure this method is on the IHostBuilder
                await host.RunAsync();
			}
			catch (Exception ex)
			{
				if (Log.Logger.GetType().Name == "SilentLogger")
				{
					Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Debug()
						.WriteTo.Console()
						.CreateLogger();
				}

				Log.Fatal(ex, "Host terminated unexpectedly");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}
