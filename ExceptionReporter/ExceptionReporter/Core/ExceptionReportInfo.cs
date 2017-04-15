using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

#pragma warning disable 1591

namespace ExceptionReporter.Core
{
	/// <summary>
    /// a bag of information (some of which is stored and retrieved from config)
    /// </summary>
    public class ExceptionReportInfo : Disposable
	{

		private readonly List<Exception> _exceptions = new List<Exception>();

        /// <summary>
        /// The Main (ostensibly 'only') exception, which is the subject of this exception 'report'
        /// Setting this property will clear any previously set exceptions
        /// <remarks>If multiple top-level exceptions are required, use SetExceptions instead</remarks>
        /// </summary>
        public Exception MainException
        {
            get { return _exceptions.Count > 0 ? _exceptions[0] : null; }
            set
            {
                _exceptions.Clear();
                _exceptions.Add(value);
            }
        }

        public IList<Exception> Exceptions
        {
            get { return _exceptions.AsReadOnly(); }
        }

        /// <summary>
        /// Add multiple exceptions to be shown (each in a separate tab if shown in dialog)
        /// <remarks>
        /// Note: Showing multiple exceptions is a special-case requirement - for only 1 top-level exception
        /// use the MainException property instead
        /// </remarks>
        /// </summary>
        public void SetExceptions(IEnumerable<Exception> exceptions)
        {
            _exceptions.Clear();
            _exceptions.AddRange(exceptions);
        }

        public string CustomMessage { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFromAddress { get; set; }
        public string SmtpServer { get; set; }

        /// <summary>
        /// an email that is displayed in the 'Contact Information' (see EmailReportAddress for the email used to actually send)
        /// </summary>
        public string ContactEmail { get; set; }

        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string RegionInfo { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public DateTime ExceptionDate { get; set; }
        public string UserExplanation { get; set; }
        public Assembly AppAssembly { get; set; }
        public string WebUrl { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string CompanyName { get; set; }

        public bool ShowGeneralTab { get; set; }
        public bool ShowConfigTab { get; set; }
        public bool ShowContactTab { get; set; }
        public bool ShowExceptionsTab { get; set; }
        public bool ShowSysInfoTab { get; set; }
        public bool ShowAssembliesTab { get; set; }

        /// <summary>
        /// address that is used to send an email (eg appears in the 'to:' field in the default email client if simpleMAPI)
        /// </summary>
        public string EmailReportAddress { get; set; }

        public string UserExplanationLabel { get; set; }
        public string ContactMessageTop { get; set; }

        public bool ShowFlatButtons { get; set; }
        public bool ShowLessMoreDetailButton{ get; set; }
        public bool ShowFullDetail { get; set; }
        public bool ShowButtonIcons { get; set; }
        public string TitleText { get; set; }

        public Color BackgroundColor { get; set; }
        public float UserExplanationFontSize { get; set; }

        public bool TakeScreenshot { get; set; }
        public Bitmap ScreenshotImage { get; set; }
        public EmailMethod MailMethod { get; set; }

        public bool ScreenshotAvailable 
        { 
            get { return TakeScreenshot && ScreenshotImage != null; }
        }


	    public ExceptionReportInfo()
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            ShowFlatButtons = true;
            ShowFullDetail = true;
            ShowButtonIcons = true;
            BackgroundColor = Color.WhiteSmoke;
            ShowExceptionsTab = true;
            ShowContactTab = false;
            ShowConfigTab = true;
            ShowAssembliesTab = true;
            ShowSysInfoTab = true;
            ShowGeneralTab = true;
            UserExplanationLabel = DefaultLabelMessages.DefaultExplanationLabel;
            ContactMessageTop = DefaultLabelMessages.DefaultContactMessageTop;
            EmailReportAddress = "support@acompany.com"; // SimpleMAPI won't work if this is blank, so show dummy place-holder
            TitleText = "Exception Report";
            UserExplanationFontSize = 12f;
            TakeScreenshot = false;
        }

        /// <summary>
        /// Enumerated type used to represent supported e-mail mechanisms 
        /// </summary>
        public enum EmailMethod
        {
            SimpleMAPI,
            SMTP
        };

        protected override void DisposeManagedResources()
        {
            if (ScreenshotImage != null)
            {
                ScreenshotImage.Dispose();
            }
            base.DisposeManagedResources();
        }
    }

    public static class DefaultLabelMessages
    {
        public const string DefaultExplanationLabel = "Please enter a brief explanation of events leading up to this exception";
        public const string DefaultContactMessageTop = "The following details can be used to obtain support for this application";
	}
}
#pragma warning restore 1591