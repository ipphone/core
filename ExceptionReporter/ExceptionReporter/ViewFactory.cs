using System;
using ContactPoint.BaseDesign;
using ExceptionReporter.Core;

namespace ExceptionReporter
{
	/// <summary>
	/// ViewFactory inspects the assembly and retrieves the appropriate class
	/// </summary>
	internal static class ViewFactory
	{
		public static T Create<T>(ViewResolver viewResolver, ExceptionReportInfo reportInfo) where T : class
		{
		    if (SyncUi.InvokeRequired)
		    {
		        return (T)SyncUi.Invoke(new Func<ViewResolver, ExceptionReportInfo, T>(Create<T>), viewResolver, reportInfo);
		    }

			var view = viewResolver.Resolve<T>();

			var constructor = view.GetConstructor(new[] {typeof (ExceptionReportInfo)});
			var newInstance = constructor.Invoke(new object[] {reportInfo});
			return newInstance as T;
		}

		public static T Create<T>(ViewResolver viewResolver) where T : class
		{
			return Activator.CreateInstance(viewResolver.Resolve<T>()) as T;
		}
	}
}