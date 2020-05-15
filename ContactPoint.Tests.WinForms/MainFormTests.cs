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
        public WindowsDriver<WindowsElement> ProcessWindow { get; set; }
        public WindowsElement MainForm { get; set; }
        public AppiumLocalService AppiumService { get; set; }

        [TestInitialize]
        public void TestSetup()
        {
            var binPath = Environment.GetEnvironmentVariable("BIN_PATH") ?? Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "..", "Binaries", "net472"));
            var waitStartupTimeout = Int32.Parse(Environment.GetEnvironmentVariable("WAIT_STARTUP_TIMEOUT") ?? "15");

            var appOpts = new AppiumOptions();
            appOpts.AddAdditionalCapability("app", Path.Combine(binPath, "contactpoint.exe"));
            appOpts.AddAdditionalCapability("appArguments", "/DisableSettingsFormAutoStartup /DisableSplashScreen");
            appOpts.AddAdditionalCapability("appWorkingDir", binPath);

            var serviceUrl = Environment.GetEnvironmentVariable("APPIUM_URL");
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                ProcessWindow = new WindowsDriver<WindowsElement>(new Uri(serviceUrl), appOpts);
            }
            else
            {
                AppiumService = AppiumLocalService.BuildDefaultService();
                AppiumService.Start();

                ProcessWindow = new WindowsDriver<WindowsElement>(AppiumService, appOpts);
            }

            try
            {
                MainForm = ProcessWindow.FindElementByXPath("//Window[@Name=\"IP PHONE\"][@AutomationId=\"MainForm\"]");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (MainForm == null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(waitStartupTimeout));
                foreach (var hndl in ProcessWindow.WindowHandles)
                {
                    try
                    {
                        ProcessWindow.SwitchTo().Window(hndl);

                        MainForm = ProcessWindow.FindElementByXPath("//Window[@Name=\"IP PHONE\"][@AutomationId=\"MainForm\"]");
                        if (MainForm != null) break;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ProcessWindow?.Close();
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
