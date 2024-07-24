using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using Quartz;
using Serilog;
using CsvHelper;
using System.Globalization;

namespace Praktika_Worker_Service
{
	public class WindowsBackgroundService : BackgroundService
	{
		private readonly Serilog.ILogger logger; //microsofto loggeri naudot
		private readonly ISchedulerFactory schedulerFactory;
		private IScheduler scheduler;

		public WindowsBackgroundService(Serilog.ILogger logger, ISchedulerFactory schedulerFactory)
		{
			this.logger = logger;
			this.schedulerFactory = schedulerFactory;
			logger.Information("Service1 instance created.");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			logger.Information("Windows Background Service started.");

			scheduler = await schedulerFactory.GetScheduler();
			await scheduler.Start();
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await scheduler.Shutdown(cancellationToken);
			logger.Information("Windows Background Service stopped.");
		}
	}

	[DisallowConcurrentExecution]
	public class ExtractInformationJob : IJob
	{
		private readonly Serilog.ILogger logger;
		private readonly PraktikaService praktikaService;

		public ExtractInformationJob(Serilog.ILogger logger, PraktikaService praktikaService)
		{
			this.logger = logger;
			this.praktikaService = praktikaService;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			logger.Information("ExtractInformationJob started.");

			try
			{
				string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
				string filePathCSV = $@"C:\Users\augustas cesnavicius\Desktop\Exceliai\CSV_{timeStamp}.csv";
				string filePathExcel = $@"C:\Users\augustas cesnavicius\Desktop\Exceliai\Excel_{timeStamp}.xlsx";
				DataTable dataTable = praktikaService.ReadFromDatabase();

				praktikaService.ConvertToCSV(dataTable, filePathCSV);
				praktikaService.ConvertToExcel(dataTable, filePathExcel);

				logger.Information("Information has been taken and converted.");

				praktikaService.SendBssNotif();
				logger.Information("BSS notification sent.");
			}
			catch (Exception ex)
			{
				logger.Error(ex, "An error occurred in ExtractInformationJob.");
			}

			await Task.CompletedTask;
		}
	}
}
