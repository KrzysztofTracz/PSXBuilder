using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public enum Verbosity
    {
        Debug = 0,
        Release,
        Mute,
        Default = Release
    }

    public interface ILogger
    {
        void Log(String text);
        void Log(String format, params object[] objects);

        void Log(Verbosity verbosity, String text);
        void Log(Verbosity verbosity, String format, params object[] objects);

        void Log(Exception exception);
        void Log(Verbosity verbosity, Exception exception);
    }

    public class Logger : ILogger, IDisposable
    {
        public const String LogDirectoryName = "log";
        public const String LogFileExtension = "txt";
        public const int    MaxLogFiles      = 10;

        public Verbosity Verbosity { get; set; }

        public Logger()
        {
            ClearLogFiles();
            OpenFileStream();
#if DEBUG
            Verbosity = Verbosity.Debug;
#else
            Verbosity = Verbosity.Default;
#endif
        }

        public void Log(string text)
        {
            Log(Verbosity.Default, text);
        }

        public void Log(String format, params object[] objects)
        {
            Log(Verbosity.Default, String.Format(format, objects));
        }

        public virtual void Log(Verbosity verbosity, string text)
        {
            WriteLineToFile(GetTimestampedText(text));
        }

        public void Log(Verbosity verbosity, string format, params object[] objects)
        {
            Log(verbosity, String.Format(format, objects));
        }

        public void Log(Exception exception)
        {
            while (exception != null)
            {
                Log(Verbosity.Default, exception.Message);
                Log(Verbosity.Debug,   exception.StackTrace);
                exception = exception.InnerException;
            }
        }

        public void Log(Verbosity verbosity, Exception exception)
        {
            while (exception != null)
            {
                Log(verbosity, exception.Message);
                Log(verbosity, exception.StackTrace);
                exception = exception.InnerException;
            }
        }

        public void Dispose()
        {
            CloseFileStream();
        }

        public void Mute()
        {
            if (Verbosity != Verbosity.Mute)
            {
                _prevVerbosity = Verbosity;
                Verbosity = Verbosity.Mute;
            }
        }

        public void Unmute()
        {
            if (Verbosity == Verbosity.Mute)
            {
                Verbosity = _prevVerbosity;
            }
        }

        protected String GetTimestampedText(String text)
        {
            return String.Format("{0} >> {1}", GetTimeStamp(), text);
        }

        protected String GetTimeStamp()
        {
            var timestamp = DateTime.Now;
            return String.Format("[{5}/{4:00}/{3:00};{0:00}:{1:00}:{2:00}]",
                                 timestamp.TimeOfDay.Hours,
                                 timestamp.TimeOfDay.Minutes,
                                 timestamp.TimeOfDay.Seconds,
                                 timestamp.Date.Day,
                                 timestamp.Date.Month,
                                 timestamp.Date.Year);
        }

        protected void ClearLogFiles()
        {
            var directory = GetLogDirectory();
            if (Directory.Exists(directory))
            {
                var files = new List<String>(Directory.EnumerateFiles(directory, "*." + LogFileExtension));
                if (files.Count > MaxLogFiles)
                {
                    files.Sort((f1, f2) => { return File.GetCreationTime(f1).CompareTo(File.GetCreationTime(f2)); });
                    for (int i = 0; i < files.Count - MaxLogFiles; i++)
                    {
                        File.Delete(files[i]);
                    }
                }
            }
        }

        protected String GetLogDirectory()
        {
            return String.Format("{0}\\{1}",
                                 Utils.GetExecutionPath(),
                                 LogDirectoryName);
        }

        protected String GetLogFilePath()
        {
            return String.Format("{0}\\{1}.{2}",
                                 GetLogDirectory(), 
                                 Utils.GetAcceptableFileName(GetTimeStamp()), 
                                 LogFileExtension);
        }

        protected void OpenFileStream()
        {
            _file = Utils.CreateFile(GetLogFilePath());
        }

        protected void WriteLineToFile(String text)
        {
            var bytes = Encoding.ASCII.GetBytes(String.Format("{0}\r\n", text));
            _file.Write(bytes, 0, bytes.Length);
            _file.Flush();
        }

        protected void CloseFileStream()
        {
            _file.Close();
            _file = null;
        }

        protected FileStream _file = null;

        private Verbosity _prevVerbosity = Verbosity.Default;
    }
}
