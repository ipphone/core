using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;

namespace ContactPoint.Tests.WinForms
{
    [TestClass]
    [TestCategory("UI")]
    [TestCategory("Smoke")]
    public class MainFormTests
    {
        public WindowsDriver<WindowsElement> AppSession { get; set; }
        public WindowsElement MainForm { get; set; }
        public AppiumLocalService AppiumService { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            var binPath = Environment.GetEnvironmentVariable("BIN_PATH") ?? Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, ".."));
            var waitStartupTimeout = Environment.GetEnvironmentVariable("WAIT_STARTUP_TIMEOUT") ?? "20";
            var appArguments = Environment.GetEnvironmentVariable("APP_ARGUMENTS");

            var appOpts = new AppiumOptions();
            appOpts.AddAdditionalCapability("platformName", "Windows");
            appOpts.AddAdditionalCapability("deviceName", "WindowsPC");
            appOpts.AddAdditionalCapability("ms:experimental-webdriver", true);
            appOpts.AddAdditionalCapability("ms:waitForAppLaunch", waitStartupTimeout);
            appOpts.AddAdditionalCapability("app", Path.Combine(binPath, "contactpoint.exe"));
            appOpts.AddAdditionalCapability("appArguments", $"/DisableSettingsFormAutoStartup /DisableSplashScreen ${appArguments}");
            appOpts.AddAdditionalCapability("appWorkingDir", binPath);

            var serviceUrl = Environment.GetEnvironmentVariable("APPIUM_URL");
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                AppSession = new WindowsDriver<WindowsElement>(new Uri(serviceUrl), appOpts);
            }
            else
            {
                AppiumService = AppiumLocalService.BuildDefaultService();
                AppiumService.Start();

                AppSession = new WindowsDriver<WindowsElement>(AppiumService, appOpts);
            }

            try
            {
                MainForm = AppSession.FindElementByXPath("//Window[@Name=\"IP PHONE\"][@AutomationId=\"MainForm\"]");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (MainForm == null)
            {
                foreach (var hndl in AppSession.WindowHandles)
                {
                    try
                    {
                        AppSession.SwitchTo().Window(hndl);

                        MainForm = AppSession.FindElementByXPath("//Window[@Name=\"IP PHONE\"][@AutomationId=\"MainForm\"]");
                        if (MainForm != null) break;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }

                AppSession.LaunchApp();
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            AppSession?.Close();
            AppiumService?.Dispose();
        }

        [TestMethod]
        public void MainForm_Loaded()
        {
            Assert.IsNotNull(MainForm, "Cannot find MainForm window");
            Assert.IsTrue(MainForm.Displayed, "MainForm isn't visible");
        }
    }
}
