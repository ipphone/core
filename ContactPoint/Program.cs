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
	/// <summary>
	/// Structure required to be sent with the WM_COPYDATA message
	/// This structure is used to contain the CommandLine
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class COPYDATASTRUCT
	{
		public IntPtr dwData = new IntPtr(3);//32 bit int to passed. Not used.
		public int cbData;//length of string. Will be one greater because of null termination.
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpData;//string to be passed.

		public COPYDATASTRUCT()
		{ }

		public COPYDATASTRUCT(string data)
		{
			lpData = data + "\0"; //add null termination
			cbData = lpData.Length; //length includes null chr so will be one greater
		}
	}

    static class Program
    {
        private const string mutex_id = "Global\\{9D643C78-280A-4455-8E3B-55E2043739ED}";

        private static LoaderForm _loaderForm;
        private static bool _disableExceptionReporter;

    	public const uint MAKECALL_MESSAGE_ID = 4890271;
		public const uint WM_COPYDATA = 0x4A;

		[DllImport("user32.dll")]
		private static extern int SendMessage(
			  IntPtr hWnd,      // handle to destination window
			  UInt32 msg,       // message
			  uint wParam,  // first message parameter
			  COPYDATASTRUCT lParam   // second message parameter
			  );

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static MainFormApplicationContext AppContext { get; private set; }
        public static ExceptionReporter.ExceptionReporter ExceptionReporter { get; private set; }

        /// <summary> 
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // Permit unmanaged code permissions
            var perm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

            // The method itself is attached with the security permission 
            // Deny for unmanaged code, which will override
            // the Assert permission in this stack frame.
            perm.Assert();

#if DEBUG
            System.Windows.Forms.Timer watchdogTimer;
#endif

            ThreadPool.SetMaxThreads(50, 200);
            bool mutexAquired = false;
            using (var mutex = new Mutex(false, mutex_id))
            {
                var logLevel = 0;
#if DEBUG
                logLevel = 5;
#endif
                var args = Environment.GetCommandLineArgs();

                var logLevelStr = GetCommandLineParameter("/loglevel", args);
                if (!string.IsNullOrEmpty(logLevelStr)) int.TryParse(logLevelStr, NumberStyles.Any, null, out logLevel);

                Logger.LogLevel = logLevel;

                // Parse logger parameter
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

                if (GetCommandLineSwitch("/DisableExceptionReporter", args))
                {
                    _disableExceptionReporter = true;
                }

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
                        Environment.Exit(0);
                    }

                    using (AppContext = new MainFormApplicationContext())
                    {
                        PartLoading("Initialize Splash Screen UI");
                        _loaderForm = new LoaderForm();
                        ThreadPool.QueueUserWorkItem(StartLoaderForm);
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
                        PartLoading("Initialize WPF Infrastructure");

                        // Create WPF application in order to let WPF controls work correctly
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

                                    PartLoading("Configuring Windows Forms UI");
                                    AppContext.ContactPointForm.Core = core;
                                    AppContext.ContactPointForm.CallOnStartup = makeCallMessage;

                                    AppContext.ContactPointForm.Shown += MainFormShown;

                                    PartLoading("Starting Windows Forms UI");
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

        static void StartLoaderForm(object obj)
        {
#if !DEBUG
            _loaderForm.ShowDialog();
#endif
        }

        static void MainFormShown(object sender, EventArgs e)
        {
            try
            {
                _loaderForm?.TryClose();
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        static void PartLoading(string obj)
        {
            Logger.LogNotice("Part loading: " + obj);

            if (_loaderForm != null && _loaderForm.IsHandleCreated && _loaderForm.IsAccessible)
            {
                _loaderForm.SetLoadingText(obj);
            }
        }

        static void LoadingFailed(Exception e)
        {
            CatchUnhandledException(null, e, "Loading failed!");

            Environment.Exit(0);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            CatchUnhandledException(sender, e.Exception, "Application Thread exception from '{0}'");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CatchUnhandledException(sender, e.ExceptionObject as Exception, "Unhandled exception from '{0}'");

            if (e.IsTerminating && !Debugger.IsAttached && MessageBox.Show(@"Try to restart application?", @"Critical error", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
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
                _loaderForm?.TryClose();
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

            if (!showExceptionReporter || _disableExceptionReporter || ExceptionReporter == null || !Application.MessageLoop)
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
