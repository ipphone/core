using System;
using System.Diagnostics;
using System.IO;
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

        [TestInitialize]
        public void TestSetup()
        {
            var binPath = Environment.GetEnvironmentVariable("BIN_PATH") ?? Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "Binaries"));

            var appOpts = new AppiumOptions();
            appOpts.AddAdditionalCapability("app", Path.Combine(binPath, "contactpoint.exe"));
            appOpts.AddAdditionalCapability("appWorkingDir", binPath);

            var serviceUrl = Environment.GetEnvironmentVariable("APPIUM_URL");
            if (string.IsNullOrEmpty(serviceUrl))
            {
                var appiumService = new AppiumServiceBuilder()
                    .WithIPAddress("127.0.0.1")
                    .UsingAnyFreePort()
                    .Build();

                appiumService.Start();

                serviceUrl = appiumService.ServiceUrl.ToString();
            }

            ProcessWindow = new WindowsDriver<WindowsElement>(new Uri(serviceUrl), appOpts);
            foreach (var hndl in ProcessWindow.WindowHandles)
            {
                ProcessWindow.SwitchTo().Window(hndl);

                try
                {
                    MainForm = ProcessWindow.FindElementByXPath("//Window[@Name=\"IP PHONE\"][@AutomationId=\"MainForm\"]");
                    if (MainForm != null) break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ProcessWindow?.Close();
        }

        [TestMethod]
        public void MainForm_Loaded()
        {
            Assert.IsNotNull(MainForm, "Cannot find MainForm window");
            Assert.IsTrue(MainForm.Displayed, "MainForm isn't visible");
        }
    }
}
