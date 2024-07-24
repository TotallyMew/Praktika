using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Praktika_Worker_Service
{
	public static class QuartzExtensions
	{
		private const string DefaultQuartzConfigSectionPath = "Quartz";
		private const string DefaultCronExpression = "0 0/1 * 1/1 * ? *"; // Fire every minute

		public static IServiceCollection AddQuartzJob<TJob>(this IServiceCollection services, HostBuilderContext context, string? configSectionPath = null) where TJob : IJob
		{
			var jobName = typeof(TJob).Name;
			var jobKey = new JobKey(jobName);

			configSectionPath ??= $"{DefaultQuartzConfigSectionPath}:{jobName}";
			var cronExpression = context.Configuration.GetValue<string>($"{configSectionPath}:CronExpression", DefaultCronExpression);
			var triggerOnStartup = context.Configuration.GetValue<bool>($"{configSectionPath}:TriggerOnStartup");

			services.AddQuartz(q =>
			{
				q.AddJob<TJob>(opts => opts.WithIdentity(jobKey))
					.AddTrigger(options => options.ForJob(jobKey)
						.WithIdentity($"{jobName}-trigger")
						.WithCronSchedule(cronExpression, builder => builder.WithMisfireHandlingInstructionIgnoreMisfires()));

				if (triggerOnStartup)
				{
					q.AddTrigger(options => options.ForJob(jobKey)
						.WithIdentity($"{jobName}-startup")
						.StartNow());
				}
			});

			return services;
		}
	}
}
