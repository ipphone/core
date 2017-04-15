using System.Text;

namespace ExceptionReporter.Core
{
    /// <summary>
    /// Encapsulates the concept of an ExceptionReport
    /// </summary>
    public class ExceptionReport
    {
        private readonly StringBuilder _reportString;

        public ExceptionReportInfo ReportInfo { get; private set; }

		/// <summary>
		/// Construct an ExceptionReport from a StringBuilder
		/// </summary>
        public ExceptionReport(StringBuilder stringBuilder, ExceptionReportInfo reportInfo)
        {
            _reportString = stringBuilder;

            ReportInfo = reportInfo;
        }

        public override string ToString()
        {
            return _reportString.ToString();
        }

    	private bool Equals(ExceptionReport obj)
        {
            return Equals(obj._reportString.ToString(), _reportString.ToString());
        }

        public override bool Equals(object obj)
        {
            return Equals((ExceptionReport) obj);
        }

        public override int GetHashCode()
        {
            return _reportString.GetHashCode();
        }
    }
}