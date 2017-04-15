using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using ExceptionReporter.Core;

namespace ExceptionReporter.Config
{
	/// <summary>
	/// Read ExceptionReport configuration from the main application's (ie not this assembly's) config file
	/// </summary>
    public class ConfigReader
    {
        const string SMTP = "SMTP";

        private readonly ExceptionReportInfo _info;

		/// <param name="reportInfo">the ExceptionReportInfo object to fill with configuration information</param>
        public ConfigReader(ExceptionReportInfo reportInfo)
        {
            _info = reportInfo;
        }

        private static string GetMailSetting(string configName)
        {
            return GetConfigSetting("Email", configName);
        }

        private static string GetContactSetting(string configName)
        {
            return GetConfigSetting("Contact", configName);
        }

        private static string GetTabSetting(string configName)
        {
            return GetConfigSetting("TabsToShow", configName);
        }

        private static string GetLabelSetting(string configName)
        {
            return GetConfigSetting("LabelMessages", configName);
        }

        private static string GetUserInterfaceSetting(string configName)
        {
            return GetConfigSetting("UserInterface", configName);
        }

        private static string GetConfigSetting(string sectionName, string configName)
        {
            var sections = ConfigurationManager.GetSection(@"ExceptionReporter/" + sectionName) as NameValueCollection;
            if (sections == null) return string.Empty;

            return sections[configName];
        }

        /// <summary>Read all settings from the application config file</summary>
        public void ReadConfig()
        {
            ReadContactSettings();
            ReadTabSettings();
            ReadMailSettings();
            ReadLabelSettings();
            ReadUserInterfaceSettings();
        }

        private void ReadContactSettings()
        {
            _info.ContactEmail = ExceptionReporterExtensions.GetString(GetContactSetting("email"), _info.ContactEmail);
            _info.WebUrl = ExceptionReporterExtensions.GetString(GetContactSetting("web"), _info.WebUrl);
            _info.Phone = ExceptionReporterExtensions.GetString(GetContactSetting("phone"), _info.Phone);
            _info.Fax = ExceptionReporterExtensions.GetString(GetContactSetting("fax"), _info.Fax);
            _info.CompanyName = ExceptionReporterExtensions.GetString(GetContactSetting("CompanyName"), _info.CompanyName);
        }

        private void ReadTabSettings()
        {
            _info.ShowExceptionsTab = ExceptionReporterExtensions.GetBool(GetTabSetting("exceptions"), _info.ShowExceptionsTab);
            _info.ShowAssembliesTab = ExceptionReporterExtensions.GetBool(GetTabSetting("assemblies"), _info.ShowAssembliesTab);
            _info.ShowConfigTab = ExceptionReporterExtensions.GetBool(GetTabSetting("config"), _info.ShowConfigTab);
            _info.ShowSysInfoTab = ExceptionReporterExtensions.GetBool(GetTabSetting("system"), _info.ShowSysInfoTab);
            _info.ShowContactTab = ExceptionReporterExtensions.GetBool(GetTabSetting("contact"), _info.ShowContactTab);
        }

        private void ReadMailSettings()
        {
            ReadMailMethod();
            ReadMailValues();
        }

        private void ReadMailValues()
        {
            _info.SmtpServer = ExceptionReporterExtensions.GetString(GetMailSetting("SmtpServer"), _info.SmtpServer);
            _info.SmtpUsername = ExceptionReporterExtensions.GetString(GetMailSetting("SmtpUsername"), _info.SmtpUsername);
            _info.SmtpPassword = ExceptionReporterExtensions.GetString(GetMailSetting("SmtpPassword"), _info.SmtpPassword);
            _info.SmtpFromAddress = ExceptionReporterExtensions.GetString(GetMailSetting("from"), _info.SmtpFromAddress);
            _info.EmailReportAddress = ExceptionReporterExtensions.GetString(GetMailSetting("to"), _info.EmailReportAddress);
        }

        private void ReadMailMethod()
        {
            var mailMethod = GetMailSetting("method");
            if (string.IsNullOrEmpty(mailMethod)) return;

            _info.MailMethod = mailMethod.Equals(SMTP) ? ExceptionReportInfo.EmailMethod.SMTP : 
														 ExceptionReportInfo.EmailMethod.SimpleMAPI;
        }

        private void ReadLabelSettings()
        {
            _info.UserExplanationLabel = ExceptionReporterExtensions.GetString(GetLabelSetting("explanation"), _info.UserExplanationLabel);
            _info.ContactMessageTop = ExceptionReporterExtensions.GetString(GetLabelSetting("ContactTop"), _info.ContactMessageTop);
        }

        private void ReadUserInterfaceSettings()
        {
            _info.ShowFlatButtons = ExceptionReporterExtensions.GetBool(GetUserInterfaceSetting("ShowFlatButtons"), _info.ShowFlatButtons);
            _info.ShowFullDetail = ExceptionReporterExtensions.GetBool(GetUserInterfaceSetting("ShowFullDetail"), _info.ShowFullDetail);

            if (!_info.ShowFullDetail)
				_info.ShowLessMoreDetailButton = true;	// prevent seeing only the simplified view, position of this line is important

            _info.ShowLessMoreDetailButton = ExceptionReporterExtensions.GetBool(GetUserInterfaceSetting("ShowLessMoreDetailButton"), _info.ShowLessMoreDetailButton);
            _info.ShowButtonIcons = ExceptionReporterExtensions.GetBool(GetUserInterfaceSetting("ShowButtonIcons"), _info.ShowButtonIcons);
            _info.TitleText = ExceptionReporterExtensions.GetString(GetUserInterfaceSetting("TitleText"), _info.TitleText);
            _info.TakeScreenshot = ExceptionReporterExtensions.GetBool(GetUserInterfaceSetting("TakeScreenshot"), _info.TakeScreenshot);

            float fontSize;
            var fontSizeAsString = GetUserInterfaceSetting("UserExplanationFontSize");
            if (float.TryParse(fontSizeAsString, NumberStyles.Float, CultureInfo.CurrentCulture.NumberFormat, out fontSize))
            {
                _info.UserExplanationFontSize = fontSize;
            }
        }

		internal static string GetConfigFilePath()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
        }
    }
}