using CsvHelper;
using OfficeOpenXml;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;

namespace Praktika_Worker_Service
{
	public sealed class PraktikaService
	{
		private readonly string strConnectionToDatabase;

		public PraktikaService(IConfiguration configuration) //IConfig neperduot per injection, uzloadint kaip IOptions
		{
			strConnectionToDatabase = configuration.GetConnectionString("DefaultConnection")
								  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
		}

		public DataTable ReadFromDatabase()
		{
			DataTable dataTable = new DataTable();
			using (SqlConnection connection = new SqlConnection(strConnectionToDatabase))
			{
				//pakeisti query kad apimtu praieto menesio informacija pvz.: jeigu liepoos 1 d., tai info bus traukiama nuo birzelio 1 iki 30
				string query = "SELECT * FROM Notification_SmsBillingInformation";
				SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
				connection.Open();
				adapter.Fill(dataTable);
				connection.Close();
			}
			return dataTable;
		}

		public void ConvertToCSV(DataTable dataTable, string filePath)
		{
			using (var writer = new StreamWriter(filePath))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					csv.WriteField(column.ColumnName);
				}
				csv.NextRecord();

				foreach (DataRow row in dataTable.Rows)
				{
					foreach (var item in row.ItemArray)
					{
						csv.WriteField(item);
					}
					csv.NextRecord();
				}
			}
		}

		public void ConvertToExcel(DataTable dataTable, string filePath)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			using (var package = new ExcelPackage(new FileInfo(filePath)))
			{
				var workSheet = package.Workbook.Worksheets.Add("Info");
				workSheet.Cells["A1"].LoadFromDataTable(dataTable, true);

				// Apply date formatting first
				for (int i = 0; i < dataTable.Columns.Count; i++) //lievas fixas turi but geresnis budas
				{
					if (dataTable.Columns[i].DataType == typeof(DateTime))
					{
						int excelColumnIndex = i + 1; // Excel columns are 1-based
						workSheet.Column(excelColumnIndex).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss.000";
					}
				}

				// AutoFit columns after formatting
				workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

				// Additional check to ensure columns are wide enough
				for (int i = 1; i <= workSheet.Dimension.End.Column; i++)
				{
					var col = workSheet.Column(i);
					if (col.Width < 10) // Set a minimum width if AutoFit did not expand enough
					{
						col.Width = 10; // You can adjust this width as needed
					}
				}

				package.Save();
			}
		}



		public void SendBssNotif()
		{
			// Implementation for sending BSS notification
		}
	}
}
