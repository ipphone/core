using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;

namespace ContactPoint.Common
{
    public static class Logger
    {
        public enum MessageType
        {
            Notice,
            Warning,
            Error
        }

        public class LogObject
        {
            public MessageType Type { get; protected set; }
            public string Message { get; protected set; }
            public DateTime DateTime { get; protected set; }

            internal LogObject(MessageType type, DateTime dateTime, string message)
            {
                Type = type;
                DateTime = dateTime;
                Message = message;

                if (LogLevel < 2) return;

                var stackTrace = new StackTrace();
                var targetFrame = stackTrace.GetFrame(3);
                if (targetFrame != null)
                {
                    var declaringType = targetFrame.GetMethod().DeclaringType;
                    if (declaringType != null)
                    {
                        Message += string.Format("; method: {1} in class {0}", new object[] 
                        { 
                            declaringType.Name,
                            targetFrame.GetMethod().Name
                        });
                    }
                }
            }

            internal LogObject(MessageType type, string message)
                : this(type, DateTime.Now, message)
            { }

            public override string ToString()
            {
                return $"[{Type}] {DateTime}: {Message}";
            }
        }

        public class ExceptionLogObject : LogObject
        {
            public Exception Exception { get; private set; }

            public ExceptionLogObject(MessageType type, DateTime dateTime, string message, Exception ex)
                : base(type, dateTime, message)
            {
                Exception = ex;
            }

            public ExceptionLogObject(MessageType type, string message, Exception ex)
                : this(type, DateTime.Now, message, ex)
            { }

            public ExceptionLogObject(MessageType type, Exception ex)
                : this(type, ex.Message, ex)
            {
                Message = ex.Message;

                if (ex.TargetSite != null)
                    Message += " in " + ex.TargetSite.Name;
                else
                    Message += "; Stack trace: " + ex.StackTrace;
            }
        }

        public delegate void MessageLoggedDelegate(LogObject logObject);

        private static IList<LogObject> _log = new List<LogObject>();
        private static int _logLevel;

        public static event MessageLoggedDelegate MessageLogged;

        public static void LogNotice(string message)
        {
            if (LogLevel <= 0) return;

            LogMessage(new LogObject(MessageType.Notice, message));
        }

        public static void LogWarn(Exception e)
        {
            LogMessage(new ExceptionLogObject(MessageType.Warning, e));
        }

        public static void LogWarn(Exception e, string message)
        {
            LogWarn(new Exception(message, e));
        }

        public static void LogWarn(Exception e, string message, params object[] args)
        {
            LogWarn(e, String.Format(message, args));
        }

        public static void LogWarn(string message)
        {
            LogMessage(new LogObject(MessageType.Warning, message));
        }

        public static void LogError(string message)
        {
            LogMessage(new LogObject(MessageType.Error, message));
        }

        public static void LogError(Exception e)
        {
            LogMessage(new ExceptionLogObject(MessageType.Error, e));
        }

        public static IList<LogObject> Log
        {
            get { return _log; }
        }

        public static TextWriter Out { get; set; }

        public static int LogLevel 
        {
            get { return _logLevel; }
            set
            {
                _logLevel = value; 

                LogMessage(new LogObject(MessageType.Notice, "Using log level " + value));
            }
        }
       
        private static string StringCollectionToString(StringCollection col)
        {
            StringBuilder result = new StringBuilder();

            foreach (string item in col)
                result.AppendLine(item);

            return result.ToString();
        }

        private static void LogMessage(LogObject logObject)
        {
            _log.Add(logObject);

            var message = logObject.ToString();

            if (logObject.Type == MessageType.Error)
            {
                Trace.TraceError(message);
            }
            else if (logObject.Type == MessageType.Warning)
            {
                Trace.TraceWarning(message);
            }
            else
            {
                Trace.TraceInformation(message);
            }

            if (Out != null)
            {
                try
                {
                    Out.WriteLine(message);
                    Out.Flush();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Unable to write to the log output stream: {0}", e);
                    Out = null;
                }
            }

            MessageLogged?.Invoke(logObject);
        }
    }
}
