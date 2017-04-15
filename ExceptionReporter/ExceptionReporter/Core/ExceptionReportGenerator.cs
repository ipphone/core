using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using ExceptionReporter.SystemInfo;

namespace ExceptionReporter.Core
{
	/// <summary>
	/// ExceptionReportGenerator does everything that needs to happen to generate an ExceptionReport
	/// This class is the entry point to use 'ExceptionReporter' as a general-purpose exception reporter
	/// (ie use this class to create an exception report without showing a GUI/dialog)
	/// </summary>
	public class ExceptionReportGenerator : Disposable
	{
		private readonly ExceptionReportInfo _reportInfo;
		private readonly List<SysInfoResult> _sysInfoResults = new List<SysInfoResult>();

		/// <summary>
		/// Initialises some ExceptionReportInfo properties related to the application/system
		/// </summary>
		/// <param name="reportInfo">an ExceptionReportInfo, can be pre-populated with config
		/// however 'base' properties such as MachineName</param>
		public ExceptionReportGenerator(ExceptionReportInfo reportInfo)
		{
		    _reportInfo = reportInfo ?? throw new ExceptionReportGeneratorException("reportInfo cannot be null");

			_reportInfo.ExceptionDate = DateTime.UtcNow;
			_reportInfo.UserName = Environment.UserName;
			_reportInfo.MachineName = Environment.MachineName;
			_reportInfo.AppName = Application.ProductName;		// TODO Application is WPF/WinForm specific, replace
			_reportInfo.RegionInfo = Application.CurrentCulture.DisplayName;
			_reportInfo.AppVersion = Application.ProductVersion;

            if (_reportInfo.AppAssembly == null)
			    _reportInfo.AppAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
		}

		/// <summary>
		/// Create an exception report 
		/// NB This method re-uses the same information retrieved from the system on subsequent calls
		/// Create a new ExceptionReportGenerator if you need to refresh system information from the computer
		/// </summary>
		/// <returns></returns>
		public ExceptionReport CreateExceptionReport()
		{
			var sysInfoResults = GetOrFetchSysInfoResults();
			var reportBuilder = new ExceptionReportBuilder(_reportInfo, sysInfoResults);
			return reportBuilder.Build();
		}

		internal IList<SysInfoResult> GetOrFetchSysInfoResults()
		{
			if (_sysInfoResults.Count == 0)
				_sysInfoResults.AddRange(CreateSysInfoResults());

			return _sysInfoResults.AsReadOnly();
		}

		private static IEnumerable<SysInfoResult> CreateSysInfoResults()
		{
			var retriever = new SysInfoRetriever();
			var results = new List<SysInfoResult>
			              {
			              	retriever.Retrieve(SysInfoQueries.OperatingSystem).Filter(
			              		new[]
			              		{
			              			"CodeSet", "CurrentTimeZone", "FreePhysicalMemory",
			              			"OSArchitecture", "OSLanguage", "Version"
			              		}),
			              	retriever.Retrieve(SysInfoQueries.Machine).Filter(
			              		new[]
			              		{
			              			"Machine", "UserName", "TotalPhysicalMemory", "Manufacturer", "Model"
			              		}),
			              };
			return results;
		}

		protected override void DisposeManagedResources()
		{
			_reportInfo.Dispose();
			base.DisposeManagedResources();
		}
	}

	internal class ExceptionReportGeneratorException : Exception
	{
		public ExceptionReportGeneratorException(string message) : base(message)
		{ }
	}
}