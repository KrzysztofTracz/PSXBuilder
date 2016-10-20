using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public interface ILogger
    {
        void Log(String text);
        void Log(String format, params object[] objects);
    }

    public class Logger : ILogger, IDisposable
    {
        public const String LogDirectoryName = "log";
        public const String LogFileExtension = "txt";
        public const int    MaxLogFiles      = 10;

        public Logger()
        {
            ClearLogFiles();
            OpenFileStream();
        }

        public virtual void Log(string text)
        {
            WriteLineToFile(GetTimestampedText(text));
        }

        public virtual void Log(String format, params object[] objects)
        {
            Log(String.Format(format, objects));
        }

        public void Dispose()
        {
            CloseFileStream();
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
                                 Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
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
    }
}
