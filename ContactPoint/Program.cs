// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using ContactPoint.Common;
using ContactPoint.Core;
using ContactPoint.Services;
using System.Linq;
using ContactPoint.BaseDesign.Wpf.CoreDesign;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Forms.VisualStyles;
using ContactPoint.Commands;
using ContactPoint.Forms;
using ExceptionReporter.Core;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ContactPoint
{
    static class Program
    {
        private const string GlobalMutexId = "Global\\{9D643C78-280A-4455-8E3B-55E2043739ED}";

        private static LoaderForm LoaderForm;
        public static MainFormApplicationContext AppContext { get; private set; }
        public static ExceptionReporter.ExceptionReporter ExceptionReporter { get; private set; }
        public static bool DisableExceptionReporter { get; set; }

        /// <summary> 
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [LoaderOptimization(LoaderOptimization.SingleDomain)]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            if (GetCommandLineSwitch("/?", args) || GetCommandLineSwitch("/h", args) || GetCommandLineSwitch("/help", args))
            {
                PrintCommandLineParameters();
                Environment.Exit(0);
                return;
            }

            var logFile = GetCommandLineParameter("/log", args);
            if (logFile != null)
            {
                try
                {
                    Logger.Out = File.CreateText(logFile);
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, "Unable to open log file at '{0}'", logFile);
                }
            }

#if DEBUG
            Logger.LogLevel = 5;
#endif
            if (int.TryParse(GetCommandLineParameter("/loglevel", args), NumberStyles.Any, null, out var logLevel))
            {
                Logger.LogLevel = logLevel;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;
            Application.ThreadException += ApplicationThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainFirstChanceException;
            ThreadPool.SetMaxThreads(50, 200);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // Permit unmanaged code permissions
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();

            DisableExceptionReporter = GetCommandLineSwitch("/DisableExceptionReporter", args);

#if DEBUG
            System.Windows.Forms.Timer watchdogTimer;
#endif

            bool mutexAquired = false;
            using (var mutex = new Mutex(false, GlobalMutexId))
            {
                Logger.LogNotice($"ContactPoint IP Phone version: {typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
                Logger.LogNotice($"Main Thread Culture is '{Thread.CurrentThread.CurrentCulture}'");
                Logger.LogNotice($"UI Thread Culture is '{Thread.CurrentThread.CurrentUICulture}'");

#if DEBUG
                if (args.Contains("/debugger", StringComparer.InvariantCultureIgnoreCase))
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    else Debugger.Launch();
                }
#endif

#if DEBUG
                watchdogTimer = new System.Windows.Forms.Timer { Interval = 3000 };
                watchdogTimer.Tick += (s, e) => { _watcherLastActivity = DateTime.Now; };
                watchdogTimer.Start();

                _watcherTargetThread = Thread.CurrentThread;
                _watcherLastActivity = DateTime.Now;

                ThreadPool.QueueUserWorkItem(WatcherThreadFunc);
#endif

                var makeCallMessage = StartPhoneCallCommand.CreateFromCommandLine(GetCommandLineParameter("/call", args));
                try
                {
                    try
                    {
                        if (!WaitForMutex(mutex))
                        {
                            SharedFileMessageTransportHost.SendMessage(makeCallMessage);
                            Environment.Exit(0);
                            return;
                        }
                        else
                        {
                            mutexAquired = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e);
                        Environment.Exit(-1);
                    }

                    using (AppContext = new MainFormApplicationContext())
                    {
                        if (!GetCommandLineSwitch("/DisableSplashScreen", args))
                        {
                            LoaderForm = new LoaderForm();
                            ThreadPool.QueueUserWorkItem(ShowSplashScreen);
                        }

                        CoreLoader.LoadingFailed += LoadingFailed;
                        CoreLoader.PartLoading += PartLoading;

                        foreach (var assembly in typeof(Program).Assembly.GetReferencedAssemblies())
                        {
                            PartLoading($"Load dependency: {assembly.Name} v{assembly.Version}");
                            AppDomain.CurrentDomain.Load(assembly);
                        }

                        PartLoading("Initialize Exception Reporter");
                        ExceptionReporter = new ExceptionReporter.ExceptionReporter();
                        ExceptionReporter.Config.ShowFlatButtons = false;
                        ExceptionReporter.Config.ShowLessMoreDetailButton = true;
                        ExceptionReporter.Config.CompanyName = "ContactPoint";
                        ExceptionReporter.Config.ContactEmail = "bug@contactpoint.com.ua";
                        ExceptionReporter.Config.WebUrl = "http://www.contactpoint.com.ua/";
#if DEBUG
                        ExceptionReporter.Config.ShowFullDetail = true;
#else
                        ExceptionReporter.Config.ShowFullDetail = false;
#endif

                        // Create WPF application in order to let WPF controls work correctly
                        PartLoading("Initialize WPF Infrastructure");
                        System.Windows.Application wpfApp = null;
                        Window wpfWnd = null;
                        try
                        {
                            PartLoading("Create WPF Application");
                            wpfApp = new System.Windows.Application();

                            PartLoading("Create WPF Window");
                            wpfWnd = new Window
                            {
                                Visibility = Visibility.Hidden,
                                ShowInTaskbar = false,
                                Width = 1,
                                Height = 1
                            };

                            PartLoading("Core infrastructure");
                            using (var core = CoreLoader.CreateCore(AppContext.MainForm))
                            {
                                PartLoading("Audio services");
                                using (new AudioDeviceService(core))
                                {
#if DEBUG
                                    if (args.Contains("/newui"))
                                    {
                                        new PhoneWindow().Show();
                                    }
#endif

                                    PartLoading("Configuring WinForms Infrastructure");
                                    AppContext.ContactPointForm.Core = core;
                                    AppContext.ContactPointForm.CallOnStartup = makeCallMessage;
                                    AppContext.ContactPointForm.DisableSettingsFormAutoStartup = GetCommandLineSwitch("/DisableSettingsFormAutoStartup", args);

                                    AppContext.ContactPointForm.Shown += MainFormShown;

                                    PartLoading("Starting WinForms UI");
                                    Application.Run(AppContext);
                                }
                            }
                        }
                        finally
                        {
                            wpfWnd?.Close();
                            wpfApp?.Shutdown(0);
                        }
                    }
                }
                finally
                {
                    if (mutexAquired)
                    {
                        mutex.ReleaseMutex();
                    }

#if DEBUG
                    _watcherThreadShutdown = true;
                    watchdogTimer.Stop();
#endif
                }
            }
        }

        static bool WaitForMutex(Mutex mutex, bool throwOnAbandoned = false)
        {
            try
            {
                return mutex.WaitOne(500, false);
            }
            catch (AbandonedMutexException e)
            {
                Logger.LogWarn(e);
                if (!throwOnAbandoned)
                {
                    return WaitForMutex(mutex, true);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }
        }

        static bool GetCommandLineSwitch(string parameterName, string[] parameters)
        {
            return parameters.Any(p => parameterName.Equals(p, StringComparison.InvariantCultureIgnoreCase));
        }

        static string GetCommandLineParameter(string parameterName, string[] parameters)
        {
            var returnNext = false;
            foreach (var p in parameters)
            {
                if (returnNext)
                {
                    if (p.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return null;
                    }

                    return p;
                }

                if (parameterName.Equals(p, StringComparison.InvariantCultureIgnoreCase))
                {
                    returnNext = true;
                }
            }

            return null;
        }

        private static void PrintCommandLineParameters()
        {
            Console.WriteLine($@"ContactPoint IP Phone ({typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion})
Usage: ContactPoint.exe [options]

Start IP Phone application.

Options:
  /Help, /h, /?                     Show command line help.
  /Call <SIP_URI>                   Start application and initiate new SIP session after application startup
                                    OR notify already running instance to initiate new SIP session.

  /Log <FILENAME>                   Write logs into specified file.
  /LogLevel <LOGLEVEL>              Set the log level: [0-10] (default: 0).

  /DisableSplashScreen              Disable Splash Screen on application startup.
  /DisableExceptionReporter         Disable Exception Reporter for handling unhandled exceptions.
  /DisableSettingsFormAutoStartup   Disable first time Settings window automatic bring up on application startup.
");

#if DEBUG
            Console.WriteLine($@"
DEBUG mode options:
  /Debugger                         Attach debugger right after application startup.
  /NewUI                            Start application with new UI (WPF).
");
#endif
        }

        static void ShowSplashScreen(object obj)
        {
#if !DEBUG
            PartLoading("Initialize Splash Screen UI");
            LoaderForm.ShowDialog();
#endif
        }

        static void MainFormShown(object sender, EventArgs e)
        {
            try
            {
                LoaderForm?.TryClose();
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        static void PartLoading(string obj)
        {
            Logger.LogNotice("Part loading: " + obj);

            if (LoaderForm != null && LoaderForm.IsHandleCreated && LoaderForm.IsAccessible)
            {
                LoaderForm.SetLoadingText(obj);
            }
        }

        static void LoadingFailed(Exception e)
        {
            CatchUnhandledException(null, e, "Loading failed!");
            Environment.Exit(-1);
        }

        static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            CatchUnhandledException(sender, e.Exception, "Application Thread exception from '{0}'");
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CatchUnhandledException(sender, e.ExceptionObject as Exception, "Unhandled exception from '{0}'");

            if (e.IsTerminating && !Debugger.IsAttached && MessageBox.Show(@"Try to restart application?", @"Critical error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        static void CurrentDomainFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                CatchUnhandledException(sender, e.Exception, "First chance exception from '{0}'", false);
            }
        }

        static void CatchUnhandledException(object sender, Exception e, string message, bool showExceptionReporter = true)
        {
            try
            {
                LoaderForm?.TryClose();
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, message, sender);
            }

            if (e is LicenseException)
            {
                Logger.LogWarn(e);
                return;
            }

            Logger.LogError(e);

            if (!showExceptionReporter || DisableExceptionReporter || ExceptionReporter == null || !Application.MessageLoop)
            {
                return;
            }

#if DEBUG
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
#endif

            try
            {
                TakeScreenshot(ExceptionReporter);
                if (!SaveErrorReportToFile($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}report_{DateTime.Now.ToString("dd_MM_yyyy")}_{Guid.NewGuid()}.txt", ExceptionReporter.Config))
                {
                    SaveErrorReportToFile(Path.GetTempFileName(), ExceptionReporter.Config);
                }

                ExceptionReporter.Show(e);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }
        }

        static bool SaveErrorReportToFile(string path, ExceptionReportInfo reportInfo)
        {
            try
            {
                using (var reportGenerator = new ExceptionReportGenerator(reportInfo))
                {
                    var report = reportGenerator.CreateExceptionReport();
                    Logger.LogError(report.ToString());
                    File.WriteAllText(path, report.ToString());
                }

                Logger.LogNotice($"Successfully saved exception report to '{path}'");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, $"Unable to save error report to '{path}'");
            }

            return false;
        }

        static void TakeScreenshot(ExceptionReporter.ExceptionReporter reporter)
        {
            try
            {
                var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                reporter.Config.ScreenshotImage = bmpScreenshot;
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Unable to take screenshot");
            }
        }

#region Debug Thread watcher
#if DEBUG
#pragma warning disable 0618
        private static DateTime _watcherLastActivity = DateTime.Now;
        private static Thread _watcherTargetThread = null;
        private static bool _watcherThreadShutdown = false;

        static void WatcherThreadFunc(object obj)
        {
            if (_watcherTargetThread == null) return;

            while (!_watcherThreadShutdown)
            {
                if ((DateTime.Now - _watcherLastActivity).Seconds > 5 && !Debugger.IsAttached)
                {
                    var stackTrace = GetStackTrace(_watcherTargetThread);
                    Logger.LogError(String.Format("Seems that main thread is stuck! Stack trace of main thread: {0}", stackTrace.ToString()));

                    return;
                }

                Thread.Sleep(1000);
            }
        }

        static StackTrace GetStackTrace(Thread targetThread)
        {
            StackTrace stackTrace = null;
            var ready = new ManualResetEventSlim();

            new Thread(() =>
            {
                // Backstop to release thread in case of deadlock:
                ready.Set();
                Thread.Sleep(200);
                try { targetThread.Resume(); }
                catch { }
            }).Start();

            ready.Wait();
            targetThread.Suspend();

            try { stackTrace = new StackTrace(targetThread, true); }
            catch { /* Deadlock */ }
            finally
            {
                try { targetThread.Resume(); }
                catch { stackTrace = null;  /* Deadlock */  }
            }

            return stackTrace;
        }
#pragma warning restore 0618
#endif
#endregion
    }
}
