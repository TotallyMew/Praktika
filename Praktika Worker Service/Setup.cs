using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration; // Make sure to include this
using Praktika_Worker_Service;
using Quartz;
using Serilog;

namespace Praktika_Worker_Service
{
	public static class Setup
	{
		public static IHost SetupApplication(this IHostBuilder builder) =>
			builder.ConfigureLogging(logging => logging.ClearProviders())
				   .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration))
				   .ConfigureServices((hostContext, services) => ConfigureServices(hostContext, services))
				   .Build();

		private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
		{
			//services.AddHostedService<WindowsBackgroundService>(); // Register the background service

			//use httpsys, windowsservice

			services.AddSingleton<PraktikaService>(); // Register PraktikaService

			services.AddQuartz(q =>
			{
				q.UseMicrosoftDependencyInjectionJobFactory();
				q.InterruptJobsOnShutdownWithWait = true;
			});

			services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

			services.AddQuartzJob<ExtractInformationJob>(context);
		}


	}
}
