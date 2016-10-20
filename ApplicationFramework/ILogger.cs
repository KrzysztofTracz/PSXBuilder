using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public interface ILogger
    {
        void Log(String text);
    }

    public class Logger : ILogger
    {
        public virtual void Log(string text)
        {
            System.Console.WriteLine(GetTimestampedText(text));
        }

        protected String GetTimestampedText(String text)
        {
            return String.Format("{0} >> {1}", GetTimeStamp(), text);
        }

        protected String GetTimeStamp()
        {
            var timestamp = DateTime.Now;
            return String.Format("[{0}:{1}:{2}|{3}/{4}/{5}]",
                                 timestamp.TimeOfDay.Hours,
                                 timestamp.TimeOfDay.Minutes,
                                 timestamp.TimeOfDay.Seconds,
                                 timestamp.Date.Day,
                                 timestamp.Date.Month,
                                 timestamp.Date.Year);
        }
    }
}
